using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Quiz.Assignment;

public class GetQuizAssignmentsEndpoint : EndpointWithoutRequest<List<QuizAssignedUserDto>>
{
    private readonly IQuizAssignmentRepository _repository;

    public GetQuizAssignmentsEndpoint(IQuizAssignmentRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/quiz/{quizId}/assignments");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var quizIdString = Route<string>("quizId");
        if (!Guid.TryParse(quizIdString, out var quizId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var assignments = await _repository.GetByQuizIdAsync(quizId);
        Response = assignments.Select(a => new QuizAssignedUserDto
        {
            Id = a.Id.ToString(),
            UserId = a.UserId.ToString()
        }).ToList();
    }
}

public class QuizAssignedUserDto
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
}
