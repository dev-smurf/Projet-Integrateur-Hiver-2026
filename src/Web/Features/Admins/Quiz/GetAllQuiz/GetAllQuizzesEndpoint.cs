using AutoMapper;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Admins.Quiz.GetQuiz;
using IMapper = AutoMapper.IMapper;

namespace Web.Features.Admins.Quiz.GetAllQuiz;

public class GetAllQuizzesEndpoint : Endpoint<EmptyRequest, List<QuizDto>>
{
    private readonly IMapper _mapper;
    private readonly IQuizRepository _quizRepository;

    public GetAllQuizzesEndpoint(
        IMapper mapper,
        IQuizRepository quizRepository)
    {
        _mapper = mapper;
        _quizRepository = quizRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Get("quiz");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var quizzes = _quizRepository.FindAll();
        System.Console.WriteLine($"[GetAllQuizzesEndpoint] Found {quizzes.Count} quizzes");
        foreach (var quiz in quizzes)
        {
            System.Console.WriteLine($"  - Quiz: {quiz.Id} | {quiz.Titre} | Questions: {quiz.Questions?.Count ?? 0}");
        }
        var quizzesDto = _mapper.Map<List<QuizDto>>(quizzes);
        await Send.OkAsync(quizzesDto, cancellation: ct);
    }
}
