using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Common;
using IMapper = AutoMapper.IMapper;

namespace Web.Features.Admins.Quiz.CreateQuestion;

public class CreateQuestionEndpoint : EndpointWithSanitizedRequest<CreateQuestionRequest, SucceededOrNotResponse>
{
    private readonly IMapper _mapper;
    private readonly IQuizRepository _quizRepository;

    public CreateQuestionEndpoint(
        IMapper mapper,
        IQuizRepository quizRepository)
    {
        _mapper = mapper;
        _quizRepository = quizRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Post("quiz/{quizId}/questions");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CreateQuestionRequest req, CancellationToken ct)
    {
        // Verify quiz exists
        var quiz = _quizRepository.FindById(req.QuizId);
        if (quiz == null)
            ThrowError("Quiz not found");

        // Create question
        var question = new QuizQuestion
        {
            QuizId = req.QuizId,
            QuestionText = req.QuestionText,
            Order = req.Order,
            QuestionType = req.QuestionType,
            Placeholder = req.Placeholder,
            ScaleMinLabel = req.ScaleMinLabel,
            ScaleMidLabel = req.ScaleMidLabel,
            ScaleMaxLabel = req.ScaleMaxLabel
        };

        // Add responses
        foreach (var responseReq in req.Responses)
        {
            question.Responses.Add(new QuizQuestionResponse
            {
                ResponseText = responseReq.ResponseText,
                Order = responseReq.Order
            });
        }

        question.SanitazeForSaving();
        quiz.Questions.Add(question);

        await _quizRepository.Update(quiz);
        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }
}
