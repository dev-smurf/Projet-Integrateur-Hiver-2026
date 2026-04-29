using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Web.Features.Members.Quiz.SubmitQuiz;

public class SubmitQuizResponseEndpoint : Endpoint<SubmitQuizRequest, SubmitQuizResponse>
{
    private readonly IUserQuizResponseRepository _userResponseRepository;
    private readonly IQuizAssignmentRepository _assignmentRepository;

    public SubmitQuizResponseEndpoint(
        IUserQuizResponseRepository userResponseRepository,
        IQuizAssignmentRepository assignmentRepository)
    {
        _userResponseRepository = userResponseRepository;
        _assignmentRepository = assignmentRepository;
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


        var assignment = await _assignmentRepository.GetAvailableByIdForUserAsync(req.QuizAssignmentId, userId);
        if (assignment == null)
            throw new KeyNotFoundException($"Quiz assignment with ID {req.QuizAssignmentId} not found");

        var question = assignment.Quiz.Questions
            .FirstOrDefault(q => q.Id == req.QuizQuestionId);

        if (question == null)
            throw new KeyNotFoundException($"Question with ID {req.QuizQuestionId} not found");

        ValidateResponse(req, question);

        var existingResponse = await _userResponseRepository.GetByAssignmentAndQuestionAsync(userId, req.QuizAssignmentId, req.QuizQuestionId);

        if (existingResponse != null)
        {
            ApplyResponse(existingResponse, req, question);
            await _userResponseRepository.UpdateAsync(existingResponse);
        }
        else
        {
            var userResponse = new UserQuizResponse
            {
                UserId = userId,
                QuizAssignmentId = req.QuizAssignmentId,
                QuizQuestionId = req.QuizQuestionId
            };

            ApplyResponse(userResponse, req, question);
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
        var selectedResponseIds = NormalizeSelectedResponseIds(req.SelectedResponseIds);

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

            case QuizQuestionType.MultipleSelection:
                if (selectedResponseIds.Count == 0)
                    throw new InvalidOperationException("At least one response selection is required for multiple selection question");

                var validResponseIds = question.Responses.Select(r => r.Id).ToHashSet();
                if (selectedResponseIds.Any(id => !validResponseIds.Contains(id)))
                    throw new InvalidOperationException("One or more selected responses are not valid for this question");
                break;

            case QuizQuestionType.TextInput:
                if (string.IsNullOrWhiteSpace(req.SelectedTextResponse))
                    throw new InvalidOperationException("Text response required for text input question");
                break;

            default:
                throw new InvalidOperationException("Unknown question type");
        }
    }

    private static void ApplyResponse(UserQuizResponse userResponse, SubmitQuizRequest req, QuizQuestion question)
    {
        userResponse.SelectedScore = null;
        userResponse.SelectedResponseId = null;
        userResponse.SelectedResponseIds = null;
        userResponse.SelectedTextResponse = null;

        switch (question.QuestionType)
        {
            case QuizQuestionType.Scale1To10:
                userResponse.SelectedScore = req.SelectedScore;
                break;
            case QuizQuestionType.MultipleChoice:
                userResponse.SelectedResponseId = req.SelectedResponseId;
                break;
            case QuizQuestionType.MultipleSelection:
                userResponse.SelectedResponseIds = string.Join(",", NormalizeSelectedResponseIds(req.SelectedResponseIds));
                break;
            case QuizQuestionType.TextInput:
                userResponse.SelectedTextResponse = req.SelectedTextResponse;
                break;
        }
    }

    private static List<Guid> NormalizeSelectedResponseIds(List<Guid>? selectedResponseIds)
    {
        return selectedResponseIds?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList() ?? [];
    }
}
