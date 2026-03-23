using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Common;

namespace Web.Features.Admins.Quiz.UpdateQuestion;

public class UpdateQuestionEndpoint : EndpointWithSanitizedRequest<UpdateQuestionRequest, SucceededOrNotResponse>
{
    private readonly IQuizRepository _quizRepository;

    public UpdateQuestionEndpoint(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Put("quiz/questions/{questionId}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UpdateQuestionRequest req, CancellationToken ct)
    {
        var allQuizzes = _quizRepository.FindAll();
        var quiz = allQuizzes.FirstOrDefault(q => q.Questions.Any(qu => qu.Id == req.QuestionId));

        if (quiz == null)
            ThrowError("Question not found");

        var question = quiz.Questions.FirstOrDefault(q => q.Id == req.QuestionId);
        if (question == null)
            ThrowError("Question not found");

        question.QuestionText = req.QuestionText;
        question.Order = req.Order;
        question.QuestionType = req.QuestionType;
        question.Placeholder = req.Placeholder;
        question.SanitazeForSaving();

        question.Responses.Clear();
        foreach (var responseReq in req.Responses)
        {
            question.Responses.Add(new QuizQuestionResponse
            {
                ResponseText = responseReq.ResponseText,
                Order = responseReq.Order
            });
        }

        await _quizRepository.Update(quiz);
        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }
}
