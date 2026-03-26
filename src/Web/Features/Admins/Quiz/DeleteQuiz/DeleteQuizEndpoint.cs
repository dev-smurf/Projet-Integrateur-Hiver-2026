using Application.Interfaces.Services;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Quiz.DeleteQuiz;

public class DeleteQuizEndpoint : Endpoint<DeleteQuizRequest, EmptyResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IHttpContextUserService _httpContextUserService;

    public DeleteQuizEndpoint(
        IQuizRepository quizRepository,
        IHttpContextUserService httpContextUserService)
    {
        _quizRepository = quizRepository;
        _httpContextUserService = httpContextUserService;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Delete("quiz/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(DeleteQuizRequest req, CancellationToken ct)
    {
        var quiz = _quizRepository.FindById(req.Id);
        quiz.SoftDelete(_httpContextUserService.Username);
        await _quizRepository.Update(quiz);
        await Send.NoContentAsync(ct);
    }
}
