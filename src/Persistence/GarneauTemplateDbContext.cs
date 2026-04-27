using System.Reflection;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Authentication;
using Domain.Entities.Books;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Persistence.Extensions;
using Persistence.Interceptors;

namespace Persistence;

public class GarneauTemplateDbContext : IdentityDbContext<User, Role, Guid,
    IdentityUserClaim<Guid>, UserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    private readonly AuditableAndSoftDeletableEntitySaveChangesInterceptor? _auditableAndSoftDeletableEntitySaveChangesInterceptor;
    private readonly AuditableEntitySaveChangesInterceptor? _auditableEntitySaveChangesInterceptor;
    private readonly UserSaveChangesInterceptor? _userSaveChangesInterceptor;
    private readonly EntitySaveChangesInterceptor? _entitySaveChangesInterceptor;

    // ✅ Constructeur unique EF Core / runtime
    public GarneauTemplateDbContext(
        DbContextOptions<GarneauTemplateDbContext> options,
        AuditableAndSoftDeletableEntitySaveChangesInterceptor? auditableAndSoftDeletableEntitySaveChangesInterceptor = null,
        AuditableEntitySaveChangesInterceptor? auditableEntitySaveChangesInterceptor = null,
        UserSaveChangesInterceptor? userSaveChangesInterceptor = null,
        EntitySaveChangesInterceptor? entitySaveChangesInterceptor = null
    ) : base(options)
    {
        _auditableAndSoftDeletableEntitySaveChangesInterceptor = auditableAndSoftDeletableEntitySaveChangesInterceptor;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _userSaveChangesInterceptor = userSaveChangesInterceptor;
        _entitySaveChangesInterceptor = entitySaveChangesInterceptor;
    }

    // ✅ DbSets
    //public DbSet<Administrator> Administrators => Set<Administrator>();
    //public DbSet<Member> Members => Set<Member>();
    public DbSet<Equipe> Equipes => Set<Equipe>();
    //public DbSet<Domain.Entities.Module> Modules => Set<Domain.Entities.Module>();
    public DbSet<Archive> Archives => Set<Archive>();
    public DbSet<Administrator> Administrators { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<MemberModule> MemberModules { get; set; } = null!;
    public DbSet<MemberModuleSectionProgress> MemberModuleSectionProgress { get; set; } = null!;
    public DbSet<Domain.Entities.Module> Modules { get; set; } = null!;
    public DbSet<ModuleSection> ModuleSections { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Conversation> Conversations { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<AdminAvailability> AdminAvailabilities { get; set; } = null!;
    public DbSet<AdminAvailabilityOverride> AdminAvailabilityOverrides { get; set; } = null!;
    public DbSet<Quiz> Quizz { get; set; } = null!;
    public DbSet<QuizQuestion> QuizQuestions { get; set; } = null!;
    public DbSet<QuizQuestionResponse> QuizQuestionResponses { get; set; } = null!;
    public DbSet<UserQuizResponse> UserQuizResponses { get; set; } = null!;
    public DbSet<QuizAssignment> QuizAssignments { get; set; } = null!;

    public GarneauTemplateDbContext()
    {
    }

    public GarneauTemplateDbContext(DbContextOptions<GarneauTemplateDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =========================
        // ✅ Fix Instant / Instant?
        // =========================
        var instantConverter = new ValueConverter<Instant, DateTime>(
            v => v.ToDateTimeUtc(),
            v => Instant.FromDateTimeUtc(DateTime.SpecifyKind(v, DateTimeKind.Utc))
        );

        var nullableInstantConverter = new ValueConverter<Instant?, DateTime?>(
            v => v.HasValue ? v.Value.ToDateTimeUtc() : null,
            v => v.HasValue ? Instant.FromDateTimeUtc(DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : null
        );

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.ClrType.GetProperties())
            {
                if (property.PropertyType == typeof(Instant))
                {
                    builder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasConversion(instantConverter)
                        .HasColumnType("datetime2");
                }

                if (property.PropertyType == typeof(Instant?))
                {
                    builder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasConversion(nullableInstantConverter)
                        .HasColumnType("datetime2");
                }
            }
        }

        // =========================
        // ✅ UserRole fix
        // =========================
        builder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // =========================
        // ✅ Book Price fix
        // =========================
        builder.Entity<Book>()
               .Property(b => b.Price)
               .HasColumnType("decimal(18,2)");

        builder.Entity<UserQuizResponse>()
               .HasOne(r => r.QuizAssignment)
               .WithMany(a => a.Responses)
               .HasForeignKey(r => r.QuizAssignmentId)
               .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // ✅ Soft delete filter
        // =========================
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                continue;

            if (entityType.ClrType == typeof(User))
                continue;

            entityType.AddSoftDeleteQueryFilter();
        }

        // =========================
        // ✅ Configurations supplémentaires
        // =========================
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_auditableAndSoftDeletableEntitySaveChangesInterceptor != null)
        {
            optionsBuilder.AddInterceptors(
                _auditableAndSoftDeletableEntitySaveChangesInterceptor,
                _auditableEntitySaveChangesInterceptor!,
                _userSaveChangesInterceptor!,
                _entitySaveChangesInterceptor!
            );
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
