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
        question.ScaleMinLabel = req.ScaleMinLabel;
        question.ScaleMidLabel = req.ScaleMidLabel;
        question.ScaleMaxLabel = req.ScaleMaxLabel;
        question.SanitazeForSaving();

        var existingResponseIds = new HashSet<Guid>(req.Responses.Where(r => r.Id.HasValue).Select(r => r.Id.Value));

        var responsesToRemove = question.Responses.Where(r => !existingResponseIds.Contains(r.Id)).ToList();
        foreach (var response in responsesToRemove)
        {
            question.Responses.Remove(response);
        }

        foreach (var responseReq in req.Responses)
        {
            if (responseReq.Id.HasValue)
            {
                var existingResponse = question.Responses.FirstOrDefault(r => r.Id == responseReq.Id.Value);
                if (existingResponse != null)
                {
                    existingResponse.ResponseText = responseReq.ResponseText;
                    existingResponse.Order = responseReq.Order;
                    existingResponse.SanitazeForSaving();
                }
            }
            else
            {
                question.Responses.Add(new QuizQuestionResponse
                {
                    ResponseText = responseReq.ResponseText,
                    Order = responseReq.Order
                });
            }
        }

        await _quizRepository.Update(quiz);
        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }
}
