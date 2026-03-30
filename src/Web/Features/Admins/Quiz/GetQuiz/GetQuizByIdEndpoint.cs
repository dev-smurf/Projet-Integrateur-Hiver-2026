using AutoMapper;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IMapper = AutoMapper.IMapper;

namespace Web.Features.Admins.Quiz.GetQuiz;

public class GetQuizByIdEndpoint : Endpoint<GetQuizByIdRequest, QuizDto>
{
    private readonly IMapper _mapper;
    private readonly IQuizRepository _quizRepository;

    public GetQuizByIdEndpoint(
        IMapper mapper,
        IQuizRepository quizRepository)
    {
        _mapper = mapper;
        _quizRepository = quizRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Get("quiz/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetQuizByIdRequest req, CancellationToken ct)
    {
        var quiz = _quizRepository.FindById(req.Id);
        var quizDto = _mapper.Map<QuizDto>(quiz);
        await Send.OkAsync(quizDto, cancellation: ct);
    }
}
