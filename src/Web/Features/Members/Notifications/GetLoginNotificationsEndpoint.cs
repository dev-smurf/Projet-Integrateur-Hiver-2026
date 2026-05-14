using Application.Interfaces.Services.Members;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Notifications;

public class GetLoginNotificationsEndpoint : EndpointWithoutRequest<LoginNotificationsResponse>
{
    private readonly IAuthenticatedMemberService _authenticatedMemberService;
    private readonly IQuizAssignmentRepository _quizAssignmentRepository;
    private readonly IMemberModuleRepository _memberModuleRepository;
    private readonly IMemberNoteRepository _memberNoteRepository;
    private readonly IEquipeNoteRepository _equipeNoteRepository;
    private readonly IEquipeRepository _equipeRepository;

    public GetLoginNotificationsEndpoint(
        IAuthenticatedMemberService authenticatedMemberService,
        IQuizAssignmentRepository quizAssignmentRepository,
        IMemberModuleRepository memberModuleRepository,
        IMemberNoteRepository memberNoteRepository,
        IEquipeNoteRepository equipeNoteRepository,
        IEquipeRepository equipeRepository)
    {
        _authenticatedMemberService = authenticatedMemberService;
        _quizAssignmentRepository = quizAssignmentRepository;
        _memberModuleRepository = memberModuleRepository;
        _memberNoteRepository = memberNoteRepository;
        _equipeNoteRepository = equipeNoteRepository;
        _equipeRepository = equipeRepository;
    }

    public override void Configure()
    {
        Get("members/me/login-notifications");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var member = _authenticatedMemberService.GetAuthenticatedMember();

        // The codebase stores Instant values as local-time-labelled-as-UTC (see InstantHelper).
        // QuizAssignment.AssignedAt, however, is set via DateTime.UtcNow (real UTC).
        // Bring both sides into the same "local clock" representation before comparing so that
        // a freshly-dismissed athlete does not keep getting toasted on every login.
        var lastSeenLocal = member.LastNotificationsSeenAt?.ToDateTimeUtc();

        var quizAssignments = await _quizAssignmentRepository.GetByUserIdAsync(member.User.Id);
        var newQuizzes = quizAssignments
            .Where(a => !a.Deleted.HasValue
                && a.CompletedAt == null
                && a.Quiz != null
                && (lastSeenLocal == null || NormalizeUtcToLocalClock(a.AssignedAt) > lastSeenLocal))
            .OrderByDescending(a => a.AssignedAt)
            .Select(a => new LoginNotificationQuizDto
            {
                AssignmentId = a.Id.ToString(),
                QuizId = a.QuizId.ToString(),
                Titre = a.Quiz!.Titre,
                FollowUpLabel = a.FollowUpLabel,
                AssignedAt = a.AssignedAt
            })
            .ToList();

        var memberModules = await _memberModuleRepository.GetByMemberIdAsync(member.Id);
        var newModules = memberModules
            .Where(mm => !mm.IsCompleted
                && (lastSeenLocal == null || mm.Created.ToDateTimeUtc() > lastSeenLocal))
            .OrderByDescending(mm => mm.Created)
            .Select(mm => new LoginNotificationModuleDto
            {
                ModuleId = mm.ModuleId.ToString(),
                Name = mm.Module.Name,
                AssignedAt = mm.Created.ToDateTimeUtc()
            })
            .ToList();

        // New Notes
        var newNotes = new List<LoginNotificationNoteDto>();
        
        // 1. Member Notes
        var memberNotes = await _memberNoteRepository.GetByMemberIdAsync(member.Id, ct);
        newNotes.AddRange(memberNotes
            .Where(n => !n.IsPrivate && (lastSeenLocal == null || n.Created.ToDateTimeUtc() > lastSeenLocal))
            .Select(n => new LoginNotificationNoteDto
            {
                Id = n.Id.ToString(),
                Content = n.Content,
                Author = n.CreatedByAdmin?.FullName ?? "Admin",
                Type = "Personnel",
                CreatedAt = n.Created.ToDateTimeUtc()
            }));

        // 2. Team Notes
        var equipeIds = await _equipeRepository.GetEquipeIdsForUser(member.User.Id);
        foreach (var equipeId in equipeIds)
        {
            var teamNotes = await _equipeNoteRepository.GetByEquipeIdAsync(equipeId, ct);
            newNotes.AddRange(teamNotes
                .Where(n => !n.IsPrivate && (lastSeenLocal == null || n.Created.ToDateTimeUtc() > lastSeenLocal))
                .Select(n => new LoginNotificationNoteDto
                {
                    Id = n.Id.ToString(),
                    Content = n.Content,
                    Author = n.CreatedByAdmin?.FullName ?? "Admin",
                    Type = "Équipe: " + (n.Equipe?.NameFr ?? ""),
                    CreatedAt = n.Created.ToDateTimeUtc()
                }));
        }

        await Send.OkAsync(new LoginNotificationsResponse
        {
            Quizzes = newQuizzes,
            Modules = newModules,
            Notes = newNotes.OrderByDescending(n => n.CreatedAt).ToList()
        }, cancellation: ct);
    }

    private static DateTime NormalizeUtcToLocalClock(DateTime utc)
    {
        var asUtc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        return DateTime.SpecifyKind(asUtc.ToLocalTime(), DateTimeKind.Utc);
    }
}

public class LoginNotificationsResponse
{
    public List<LoginNotificationQuizDto> Quizzes { get; set; } = new();
    public List<LoginNotificationModuleDto> Modules { get; set; } = new();
    public List<LoginNotificationNoteDto> Notes { get; set; } = new();
}

public class LoginNotificationQuizDto
{
    public string AssignmentId { get; set; } = null!;
    public string QuizId { get; set; } = null!;
    public string Titre { get; set; } = null!;
    public string? FollowUpLabel { get; set; }
    public DateTime AssignedAt { get; set; }
}

public class LoginNotificationModuleDto
{
    public string ModuleId { get; set; } = null!;
    public string? Name { get; set; }
    public DateTime AssignedAt { get; set; }
}
public class LoginNotificationNoteDto
{
    public string Id { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
