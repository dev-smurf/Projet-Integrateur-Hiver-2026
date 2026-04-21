using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Web.Features.Members.Quiz.SubmitQuiz;

public class SubmitQuizResponseEndpoint : Endpoint<SubmitQuizRequest, SubmitQuizResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IUserQuizResponseRepository _userResponseRepository;

    public SubmitQuizResponseEndpoint(
        IQuizRepository quizRepository,
        IUserQuizResponseRepository userResponseRepository)
    {
        _quizRepository = quizRepository;
        _userResponseRepository = userResponseRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Post("quiz/submit-response");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(SubmitQuizRequest req, CancellationToken ct)
    {
        var userIdString = User.FindFirst("userId")?.Value;
        if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out var userId))
            throw new UnauthorizedAccessException("Invalid or missing user identifier");


        var allQuizzes = _quizRepository.FindAll();
        var question = allQuizzes
            .SelectMany(q => q.Questions)
            .FirstOrDefault(q => q.Id == req.QuizQuestionId);

        if (question == null)
            throw new KeyNotFoundException($"Question with ID {req.QuizQuestionId} not found");

        ValidateResponse(req, question);

        var existingResponse = await _userResponseRepository.GetByUserAndQuestionAsync(userId, req.QuizQuestionId);

        if (existingResponse != null)
        {
            existingResponse.SelectedScore = req.SelectedScore;
            existingResponse.SelectedResponseId = req.SelectedResponseId;
            existingResponse.SelectedTextResponse = req.SelectedTextResponse;
            await _userResponseRepository.UpdateAsync(existingResponse);
        }
        else
        {
            var userResponse = new UserQuizResponse
            {
                UserId = userId,
                QuizQuestionId = req.QuizQuestionId,
                SelectedScore = req.SelectedScore,
                SelectedResponseId = req.SelectedResponseId,
                SelectedTextResponse = req.SelectedTextResponse
            };

            await _userResponseRepository.CreateAsync(userResponse);
        }

        await Send.OkAsync(new SubmitQuizResponse
        {
            QuizQuestionId = req.QuizQuestionId,
            QuestionType = question.QuestionType,
            QuestionText = question.QuestionText,
            IsValid = true,
            Message = "Response submitted successfully"
        }, cancellation: ct);
    }

    private void ValidateResponse(SubmitQuizRequest req, QuizQuestion question)
    {
        switch (question.QuestionType)
        {
            case QuizQuestionType.Scale1To10:
                if (!req.SelectedScore.HasValue || req.SelectedScore < 1 || req.SelectedScore > 10)
                    throw new InvalidOperationException("Invalid score: must be between 1 and 10");
                break;

            case QuizQuestionType.MultipleChoice:
                if (!req.SelectedResponseId.HasValue)
                    throw new InvalidOperationException("Response selection required for multiple choice question");
                var response = question.Responses.FirstOrDefault(r => r.Id == req.SelectedResponseId);
                if (response == null)
                    throw new InvalidOperationException("Selected response is not valid for this question");
                break;

            case QuizQuestionType.TextInput:
                if (string.IsNullOrWhiteSpace(req.SelectedTextResponse))
                    throw new InvalidOperationException("Text response required for text input question");
                break;

            default:
                throw new InvalidOperationException("Unknown question type");
        }
    }
}
