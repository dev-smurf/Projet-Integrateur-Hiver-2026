using AutoMapper;
using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Common;
using IMapper = AutoMapper.IMapper;

namespace Web.Features.Admins.Quiz.UpdateQuiz;

public class UpdateQuizEndpoint : EndpointWithSanitizedRequest<UpdateQuizRequest, SucceededOrNotResponse>
{
    private readonly IMapper _mapper;
    private readonly IQuizRepository _quizRepository;

    public UpdateQuizEndpoint(
        IMapper mapper,
        IQuizRepository quizRepository)
    {
        _mapper = mapper;
        _quizRepository = quizRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Put("quiz/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UpdateQuizRequest req, CancellationToken ct)
    {
        var existingQuiz = _quizRepository.FindById(req.Id);
        existingQuiz = _mapper.Map(req, existingQuiz);
        await _quizRepository.Update(existingQuiz);
        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }
}
