using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Quiz.AssignQuiz;

public class AssignQuizEndpoint : Endpoint<AssignQuizRequest, EmptyResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAssignmentRepository _assignmentRepository;

    public AssignQuizEndpoint(
        IQuizRepository quizRepository,
        IQuizAssignmentRepository assignmentRepository)
    {
        _quizRepository = quizRepository;
        _assignmentRepository = assignmentRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Post("quiz/{quizId}/assign");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(AssignQuizRequest req, CancellationToken ct)
    {
        var quiz = _quizRepository.FindById(req.QuizId);
        if (quiz == null)
            throw new KeyNotFoundException($"Quiz with ID {req.QuizId} not found");

        // Get existing assignments to avoid duplicates
        var existingAssignments = await _assignmentRepository.GetByQuizIdAsync(req.QuizId);
        var alreadyAssignedUserIds = existingAssignments
            .Where(a => !a.Deleted.HasValue)
            .Select(a => a.UserId)
            .ToHashSet();

        var newUserIds = req.UserIds
            .Where(userId => !alreadyAssignedUserIds.Contains(userId))
            .ToList();

        if (newUserIds.Count == 0)
        {
            await Send.NoContentAsync(ct);
            return;
        }

        var assignments = newUserIds
            .Select(userId => new QuizAssignment
            {
                QuizId = req.QuizId,
                UserId = userId,
                AssignedAt = DateTime.UtcNow,
                DueDate = req.DueDate
            })
            .ToList();

        if (assignments.Count > 0)
            await _assignmentRepository.CreateRangeAsync(assignments);

        await Send.NoContentAsync(ct);
    }
}
