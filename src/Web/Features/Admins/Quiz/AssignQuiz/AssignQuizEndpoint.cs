using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Quiz.AssignQuiz;

public class AssignQuizEndpoint : Endpoint<AssignQuizRequest, EmptyResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAssignmentRepository _assignmentRepository;
    private readonly IEquipeRepository _equipeRepository;

    public AssignQuizEndpoint(
        IQuizRepository quizRepository,
        IQuizAssignmentRepository assignmentRepository,
        IEquipeRepository equipeRepository)
    {
        _quizRepository = quizRepository;
        _assignmentRepository = assignmentRepository;
        _equipeRepository = equipeRepository;
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

        var equipeUserIds = new List<Guid>();
        if (req.EquipeIds.Count > 0)
        {
            var equipes = await _equipeRepository.GetByIds(req.EquipeIds);
            equipeUserIds = equipes
                .SelectMany(equipe => equipe.Membres)
                .Select(user => user.Id)
                .ToList();
        }

        var userIds = req.UserIds
            .Concat(equipeUserIds)
            .Where(userId => userId != Guid.Empty)
            .Distinct()
            .ToList();

        if (userIds.Count == 0)
        {
            await Send.NoContentAsync(ct);
            return;
        }

        var nextFollowUpOrders = await _assignmentRepository.GetNextFollowUpOrdersAsync(req.QuizId, userIds);

        var assignments = userIds
            .Select(userId => new QuizAssignment
            {
                QuizId = req.QuizId,
                UserId = userId,
                Version = nextFollowUpOrders[userId],
                FollowUpLabel = string.IsNullOrWhiteSpace(req.FollowUpLabel) ? null : req.FollowUpLabel.Trim(),
                AssignedAt = DateTime.UtcNow,
                AvailableAt = req.AvailableAt,
                DueDate = req.DueDate
            })
            .ToList();

        if (assignments.Count > 0)
            await _assignmentRepository.CreateRangeAsync(assignments);

        await Send.NoContentAsync(ct);
    }
}
