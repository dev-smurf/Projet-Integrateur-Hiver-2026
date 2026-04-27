using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Web.Features.Members.Quiz.GetAssignedQuizzes;

public class GetAssignedQuizzesEndpoint : Endpoint<EmptyRequest, List<AssignedQuizDto>>
{
    private readonly IQuizAssignmentRepository _assignmentRepository;

    public GetAssignedQuizzesEndpoint(IQuizAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Get("quiz/assigned");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirst("userId")?.Value ?? throw new UnauthorizedAccessException());

        var assignments = await _assignmentRepository.GetByUserIdAsync(userId);

        var result = assignments
            .Where(a => !a.Deleted.HasValue)
            .Select(a => new AssignedQuizDto
            {
                Id = a.Id,
                QuizId = a.QuizId,
                Version = a.Version,
                Titre = a.Quiz.Titre,
                Description = a.Quiz.Description,
                ImageUrl = a.Quiz.ImageUrl,
                AssignedAt = a.AssignedAt,
                AvailableAt = a.AvailableAt,
                DueDate = a.DueDate,
                CompletedAt = a.CompletedAt
            })
            .ToList();

        await Send.OkAsync(result, cancellation: ct);
    }
}
