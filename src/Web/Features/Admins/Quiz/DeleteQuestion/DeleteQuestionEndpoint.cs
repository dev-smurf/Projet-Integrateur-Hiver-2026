using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Common;

namespace Web.Features.Admins.Quiz.DeleteQuestion;

public class DeleteQuestionEndpoint : Endpoint<DeleteQuestionRequest, SucceededOrNotResponse>
{
    private readonly IQuizRepository _quizRepository;

    public DeleteQuestionEndpoint(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Delete("quiz/questions/{questionId}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(DeleteQuestionRequest req, CancellationToken ct)
    {
        var allQuizzes = _quizRepository.FindAll();
        var quiz = allQuizzes.FirstOrDefault(q => q.Questions.Any(qu => qu.Id == req.QuestionId));

        if (quiz == null)
            ThrowError("Question not found");

        var question = quiz.Questions.FirstOrDefault(q => q.Id == req.QuestionId);
        if (question == null)
            ThrowError("Question not found");

        quiz.Questions.Remove(question);
        await _quizRepository.Update(quiz);

        await Send.OkAsync(new SucceededOrNotResponse(true), cancellation: ct);
    }
}
