using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Quiz.Assignment;

public class GetMemberAssignedQuizzesEndpoint : EndpointWithoutRequest<List<MemberAssignedQuizDto>>
{
    private readonly IQuizAssignmentRepository _assignmentRepository;
    private readonly IQuizRepository _quizRepository;

    public GetMemberAssignedQuizzesEndpoint(
        IQuizAssignmentRepository assignmentRepository,
        IQuizRepository quizRepository)
    {
        _assignmentRepository = assignmentRepository;
        _quizRepository = quizRepository;
    }

    public override void Configure()
    {
        Get("/admin/members/{userId}/assigned-quizzes");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdString = Route<string>("userId");
        if (!Guid.TryParse(userIdString, out var userId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var assignments = await _assignmentRepository.GetByUserIdAsync(userId);

        Response = assignments.Select(a =>
        {
            var quiz = _quizRepository.FindById(a.QuizId);
            return new MemberAssignedQuizDto
            {
                Id = a.Id.ToString(),
                QuizId = a.QuizId.ToString(),
                Titre = quiz?.Titre ?? "Quiz",
                Description = quiz?.Description,
                ImageUrl = quiz?.ImageUrl,
                Version = a.Version,
                FollowUpLabel = a.FollowUpLabel,
                AssignedAt = a.AssignedAt,
                AvailableAt = a.AvailableAt,
                DueDate = a.DueDate,
                CompletedAt = a.CompletedAt
            };
        }).ToList();
    }
}

public class MemberAssignedQuizDto
{
    public string Id { get; set; } = null!;
    public string QuizId { get; set; } = null!;
    public string Titre { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int Version { get; set; }
    public string? FollowUpLabel { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? AvailableAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted => CompletedAt.HasValue;
}