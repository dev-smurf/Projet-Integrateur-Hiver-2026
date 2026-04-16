using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace Web.Features.Admins.Quiz.Assignment;

public class UnassignQuizRequest
{
    public Guid QuizId { get; set; }
    public List<Guid> UserIds { get; set; } = new();
}

public class UnassignQuizEndpoint : Endpoint<UnassignQuizRequest, EmptyResponse>
{
    private readonly IQuizAssignmentRepository _repository;

    public UnassignQuizEndpoint(IQuizAssignmentRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Post("/quiz/{quizId}/unassign");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UnassignQuizRequest req, CancellationToken ct)
    {
        var assignments = await _repository.GetQueryable().Where(a => a.QuizId == req.QuizId).ToListAsync();
        if (assignments == null || !assignments.Any())
        {
            await Send.NoContentAsync(ct);
            return;
        }

        var toDelete = assignments.Where(a => req.UserIds.Contains(a.UserId)).ToList();
        foreach (var a in toDelete)
        {
            await _repository.DeleteAsync(a.Id);
        }

        await Send.NoContentAsync(ct);
    }
}
