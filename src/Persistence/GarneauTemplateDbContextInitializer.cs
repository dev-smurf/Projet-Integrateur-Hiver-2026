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
            var pendingMigrations = (await _context.Database.GetPendingMigrationsAsync()).ToList();
            if (pendingMigrations.Count == 0)
                return;

            // If the database already has the schema (from old consolidated migrations),
            // mark the new migrations as applied instead of re-running them.
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoles'";
            var result = await command.ExecuteScalarAsync();
            var tablesAlreadyExist = result != null && Convert.ToInt32(result) > 0;

            if (tablesAlreadyExist)
            {
                // Only mark the initial migration as applied (base schema already exists)
                var initialMigration = pendingMigrations.FirstOrDefault(m => m.EndsWith("_InitialCreate"));
                if (initialMigration != null)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {0}) " +
                        "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ({0}, {1})",
                        initialMigration, "10.0.2");
                    _logger.LogInformation("Marked InitialCreate migration as applied (schema already exists).");
                }

                // Run any remaining new migrations
                await _context.Database.MigrateAsync();
            }
            else
            {
                await _context.Database.MigrateAsync();
            }
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
                NameFr = "Introduction à la programmation",
                NameEn = "Introduction to Programming",
                SujetFr = "Bases de la programmation",
                SujetEn = "Programming Basics",
                ContenueFr = "Ce module couvre les concepts fondamentaux de la programmation, incluant les variables, les boucles et les conditions.",
                ContenueEn = "This module covers fundamental programming concepts, including variables, loops, and conditions.",
                CardImageUrl = null
            },
            new Module
            {
                NameFr = "Développement Web",
                NameEn = "Web Development",
                SujetFr = "HTML, CSS et JavaScript",
                SujetEn = "HTML, CSS and JavaScript",
                ContenueFr = "Apprenez à créer des sites web modernes avec HTML5, CSS3 et JavaScript.",
                ContenueEn = "Learn to create modern websites with HTML5, CSS3 and JavaScript.",
                CardImageUrl = null
            },
            new Module
            {
                NameFr = "Bases de données",
                NameEn = "Databases",
                SujetFr = "SQL et NoSQL",
                SujetEn = "SQL and NoSQL",
                ContenueFr = "Découvrez les systèmes de gestion de bases de données relationnelles et non-relationnelles.",
                ContenueEn = "Discover relational and non-relational database management systems.",
                CardImageUrl = null
            }
        };

        _context.Modules.AddRange(modules);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} modules", modules.Length);
    }
}