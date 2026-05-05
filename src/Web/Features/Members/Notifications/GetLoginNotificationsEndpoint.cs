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

    public GetLoginNotificationsEndpoint(
        IAuthenticatedMemberService authenticatedMemberService,
        IQuizAssignmentRepository quizAssignmentRepository,
        IMemberModuleRepository memberModuleRepository)
    {
        _authenticatedMemberService = authenticatedMemberService;
        _quizAssignmentRepository = quizAssignmentRepository;
        _memberModuleRepository = memberModuleRepository;
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

        await Send.OkAsync(new LoginNotificationsResponse
        {
            Quizzes = newQuizzes,
            Modules = newModules
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
