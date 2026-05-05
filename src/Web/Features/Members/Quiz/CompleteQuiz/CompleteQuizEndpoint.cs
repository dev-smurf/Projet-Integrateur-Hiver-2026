using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Members.Quiz.CompleteQuiz;

public class CompleteQuizEndpoint : Endpoint<CompleteQuizRequest, EmptyResponse>
{
    private readonly IQuizAssignmentRepository _assignmentRepository;

    public CompleteQuizEndpoint(IQuizAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public override void Configure()
    {
        Post("quiz/assignments/{quizAssignmentId}/complete");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CompleteQuizRequest req, CancellationToken ct)
    {
        var userIdString = User.FindFirst("userId")?.Value;
        if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out var userId))
            throw new UnauthorizedAccessException("Invalid or missing user identifier");

        var assignment = await _assignmentRepository.GetAvailableByIdForUserAsync(req.QuizAssignmentId, userId);

        if (assignment == null)
            throw new KeyNotFoundException("Quiz assignment not found");

        assignment.CompletedAt = DateTime.UtcNow;
        await _assignmentRepository.UpdateAsync(assignment);

        await Send.OkAsync(ct);
    }
}

public class CompleteQuizRequest
{
    public Guid QuizAssignmentId { get; set; }
}
