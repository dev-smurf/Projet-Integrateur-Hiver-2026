using Domain.Constants.User;
using Domain.Entities;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Persistence;

public class GarneauTemplateDbContextInitializer
{
    private const string MemberEmail = "member@gmail.com";
    private const string AdminEmail = "admin@gmail.com";
    private const string TestEmail = "test@test.com";
    private const string Password = "Qwerty123!";
    private const string TestPassword = "Test12345!";

    private readonly ILogger<GarneauTemplateDbContextInitializer> _logger;
    private readonly GarneauTemplateDbContext _context;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public GarneauTemplateDbContextInitializer(ILogger<GarneauTemplateDbContextInitializer> logger,
        GarneauTemplateDbContext context,
        RoleManager<Role> roleManager,
        UserManager<User> userManager)
    {
        _logger = logger;
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // If the database already has the schema (from old consolidated migrations),
            // mark the new migrations as applied instead of re-running them.
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoles'";
            var result = await command.ExecuteScalarAsync();
            var tablesAlreadyExist = result != null && Convert.ToInt32(result) > 0;
            var allMigrations = _context.Database.GetMigrations().ToList();
            var baselineMigration = allMigrations.FirstOrDefault();

            if (tablesAlreadyExist)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory') " +
                    "BEGIN " +
                    "CREATE TABLE [__EFMigrationsHistory] ([MigrationId] nvarchar(150) NOT NULL, [ProductVersion] nvarchar(32) NOT NULL, CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])); " +
                    "END");

                await using var historyCommand = connection.CreateCommand();
                historyCommand.CommandText = "SELECT COUNT(*) FROM [__EFMigrationsHistory]";
                var historyResult = await historyCommand.ExecuteScalarAsync();
                var historyCount = historyResult != null ? Convert.ToInt32(historyResult) : 0;

                if (historyCount == 0)
                {
                    if (!string.IsNullOrWhiteSpace(baselineMigration))
                    {
                        await _context.Database.ExecuteSqlRawAsync(
                            "IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {0}) " +
                            "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ({0}, {1})",
                            baselineMigration, "10.0.2");
                        _logger.LogInformation("Marked baseline migration {Migration} as applied (schema already exists).", baselineMigration);
                    }
                }

                var needsRepair = await NeedsSchemaRepairAsync();

                if (needsRepair && !string.IsNullOrWhiteSpace(baselineMigration))
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] <> {0}",
                        baselineMigration);
                    _logger.LogWarning("Migration history repaired (removed non-baseline entries) to allow schema updates.");
                }
            }

            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedRoles();
            await SeedAdmins();
            await SeedMembers();
            await SeedTestMember();
            await SeedModules();
            await SeedQuizzes();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedRoles()
    {
        if (!_roleManager.RoleExistsAsync(Roles.ADMINISTRATOR).Result)
            await _roleManager.CreateAsync(new Role { Name = Roles.ADMINISTRATOR, NormalizedName = Roles.ADMINISTRATOR.Normalize() });

        if (!_roleManager.RoleExistsAsync(Roles.MEMBER).Result)
            await _roleManager.CreateAsync(new Role { Name = Roles.MEMBER, NormalizedName = Roles.MEMBER.Normalize() });
    }

    private async Task SeedAdmins()
    {
        var user = await _userManager.FindByEmailAsync(AdminEmail);
        if (user != null)
            return;

        user = BuildUser(AdminEmail);
        var result = await _userManager.CreateAsync(user, Password);

        if (result.Succeeded)
            await _userManager.AddToRoleAsync(user, Roles.ADMINISTRATOR);
        else
            throw new Exception($"Could not seed/create {Roles.ADMINISTRATOR} user.");


        var admin = new Administrator("Super", "Admin");
        admin.SetUser(user);
        _context.Administrators.Add(admin);
        await _context.SaveChangesAsync();
    }

    private async Task SeedMembers()
    {
        var user = await _userManager.FindByEmailAsync(MemberEmail);
        if (user != null)
            return;

        user = BuildUser(MemberEmail);
        var result = await _userManager.CreateAsync(user, Password);

        if (result.Succeeded)
            await _userManager.AddToRoleAsync(user, Roles.MEMBER);
        else
            throw new Exception($"Could not seed/create {Roles.MEMBER} user.");

        var existingMember = _context.Members.IgnoreQueryFilters().FirstOrDefault(x => x.User.Id == user.Id);
        if (existingMember is { Active: true })
            return;

        if (existingMember == null)
        {
            var member = new Member("John", "Doe", 1, "123, my street", "Quebec", "A1A 1A1");
            member.SetUser(user);
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
        }
        else if (!existingMember.Active)
        {
            existingMember.Activate();
            _context.Members.Update(existingMember);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedTestMember()
    {
        var existingUser = await _userManager.FindByEmailAsync(TestEmail);
        if (existingUser != null)
        {
            // Ensure user is not soft-deleted
            if (!existingUser.IsActive())
            {
                existingUser.Restore();
                await _userManager.UpdateAsync(existingUser);
                _logger.LogInformation("Reactivated soft-deleted test user: {Email}", TestEmail);
            }

            // Ensure member record exists
            var existingMember = _context.Members.IgnoreQueryFilters().FirstOrDefault(m => m.User.Id == existingUser.Id);
            if (existingMember == null)
            {
                var member = new Member("Test", "User", null, "123 Test St", "Test City", "A1A 1A1");
                member.SetUser(existingUser);
                _context.Members.Add(member);
                await _context.SaveChangesAsync();
            }
            else if (!existingMember.Active)
            {
                existingMember.Activate();
                _context.Members.Update(existingMember);
                await _context.SaveChangesAsync();
            }
            return;
        }

        var user = BuildUser(TestEmail);
        var result = await _userManager.CreateAsync(user, TestPassword);

        if (!result.Succeeded)
            throw new Exception($"Could not create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        await _userManager.AddToRoleAsync(user, Roles.MEMBER);

        var newMember = new Member("Test", "User", null, "123 Test St", "Test City", "A1A 1A1");
        newMember.SetUser(user);
        _context.Members.Add(newMember);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded test member user: {Email}", TestEmail);
    }

    private User BuildUser(string email)
    {
        return new User
        {
            Email = email,
            UserName = email,
            NormalizedEmail = email.Normalize(),
            NormalizedUserName = email,
            PhoneNumber = "555-555-5555",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false
        };
    }

    private async Task SeedModules()
    {
        if (_context.Modules.Any())
            return;

        var modules = new[]
        {
            new Module
            {
                Name = "Introduction à la programmation",
                Subject = "Bases de la programmation",
                Content = "Ce module couvre les concepts fondamentaux de la programmation, incluant les variables, les boucles et les conditions.",
                CardImageUrl = null
            },
            new Module
            {
                Name = "Développement Web",
                Subject = "HTML, CSS et JavaScript",
                Content = "Apprenez à créer des sites web modernes avec HTML5, CSS3 et JavaScript.",
                CardImageUrl = null
            },
            new Module
            {
                Name = "Bases de données",
                Subject = "SQL et NoSQL",
                Content = "Découvrez les systèmes de gestion de bases de données relationnelles et non-relationnelles.",
                CardImageUrl = null
            }
        };

        _context.Modules.AddRange(modules);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} modules", modules.Length);
    }

    private async Task SeedQuizzes()
    {
        if (!await TableExistsAsync("Quizz"))
        {
            _logger.LogWarning("Skipping quiz seeding because table 'Quizz' does not exist.");
            return;
        }

        if (_context.Quizz.Any())
            return;

        // Create sample quizzes with questions and responses
        var quizzes = new[]
        {
            new Quiz
            {
                Titre = "Tester vos habitudes",
                Description = "Un questionnaire sur les habitudes de travail et performance",
                ImageUrl = "https://via.placeholder.com/300x200?text=Habitudes"
            }
        };

        _context.Quizz.AddRange(quizzes);
        await _context.SaveChangesAsync();

        // Get the created quiz
        var quiz = _context.Quizz.First();

        // Create questions
        var questions = new[]
        {
            new QuizQuestion { QuizId = quiz.Id, QuestionText = "Je me parle positivement.", Order = 1, QuestionType = QuizQuestionType.Scale1To10 },
            new QuizQuestion { QuizId = quiz.Id, QuestionText = "Une mauvaise performance ne me déprime jamais.", Order = 2, QuestionType = QuizQuestionType.Scale1To10 },
            new QuizQuestion { QuizId = quiz.Id, QuestionText = "Je continue à travailler même lorsque je suis physiquement fatigué.", Order = 3, QuestionType = QuizQuestionType.Scale1To10 },
            new QuizQuestion { QuizId = quiz.Id, QuestionText = "J'ai hâte d'aller m'entraîner tous les jours.", Order = 4, QuestionType = QuizQuestionType.Scale1To10 },
            new QuizQuestion { QuizId = quiz.Id, QuestionText = "Je gère bien l'anxiété et la pression.", Order = 5, QuestionType = QuizQuestionType.Scale1To10 },
            new QuizQuestion { QuizId = quiz.Id, QuestionText = "Je m'imagine performer parfaitement.", Order = 6, QuestionType = QuizQuestionType.Scale1To10 },
            new QuizQuestion { QuizId = quiz.Id, QuestionText = "Je bloque les distractions pour pouvoir me concentrer.", Order = 7, QuestionType = QuizQuestionType.Scale1To10 },
            new QuizQuestion { QuizId = quiz.Id, QuestionText = "Je gère bien la frustration dans la pratique.", Order = 8, QuestionType = QuizQuestionType.Scale1To10 },
            new QuizQuestion { QuizId = quiz.Id, QuestionText = "Je travaille quotidiennement avec mes objectifs.", Order = 9, QuestionType = QuizQuestionType.Scale1To10 }
        };

        _context.QuizQuestions.AddRange(questions);
        await _context.SaveChangesAsync();

        // Create response columns (Jamais, Parfois, Toujours) for each question
        var allQuestions = _context.QuizQuestions.Where(q => q.QuizId == quiz.Id).ToList();
        var responses = new List<QuizQuestionResponse>();

        foreach (var question in allQuestions)
        {
            responses.Add(new QuizQuestionResponse 
            { 
                QuizQuestionId = question.Id, 
                ResponseText = "Jamais", 
                Order = 0 
            });
            responses.Add(new QuizQuestionResponse 
            { 
                QuizQuestionId = question.Id, 
                ResponseText = "Parfois", 
                Order = 1 
            });
            responses.Add(new QuizQuestionResponse 
            { 
                QuizQuestionId = question.Id, 
                ResponseText = "Toujours", 
                Order = 2 
            });
        }

        _context.QuizQuestionResponses.AddRange(responses);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} quizzes with {QCount} questions", quizzes.Length, allQuestions.Count);
    }

    private async Task<bool> TableExistsAsync(string tableName)
    {
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State != System.Data.ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @table";
        var param = command.CreateParameter();
        param.ParameterName = "@table";
        param.Value = tableName;
        command.Parameters.Add(param);
        var result = await command.ExecuteScalarAsync();
        if (shouldClose)
            await connection.CloseAsync();
        return result != null && Convert.ToInt32(result) > 0;
    }

    private async Task<bool> ColumnExistsAsync(string tableName, string columnName)
    {
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State != System.Data.ConnectionState.Open;
        if (shouldClose)
            await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @table AND COLUMN_NAME = @column";
        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@table";
        tableParam.Value = tableName;
        command.Parameters.Add(tableParam);
        var columnParam = command.CreateParameter();
        columnParam.ParameterName = "@column";
        columnParam.Value = columnName;
        command.Parameters.Add(columnParam);
        var result = await command.ExecuteScalarAsync();
        if (shouldClose)
            await connection.CloseAsync();
        return result != null && Convert.ToInt32(result) > 0;
    }

    private async Task<bool> NeedsSchemaRepairAsync()
    {
        // Core tables that must exist for current code paths
        if (!await TableExistsAsync("Modules"))
            return true;
        if (!await TableExistsAsync("MemberModules"))
            return true;
        if (!await TableExistsAsync("ModuleSections"))
            return true;
        if (!await TableExistsAsync("Messages"))
            return true;
        if (!await TableExistsAsync("Quizz"))
            return true;

        // Required renamed columns from the latest module structure
        if (!await ColumnExistsAsync("Modules", "Name"))
            return true;
        if (!await ColumnExistsAsync("Modules", "Subject"))
            return true;
        if (!await ColumnExistsAsync("Modules", "Content"))
            return true;

        return false;
    }
}
