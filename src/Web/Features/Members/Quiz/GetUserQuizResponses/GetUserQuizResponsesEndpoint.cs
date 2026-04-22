using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Web.Features.Members.Quiz.GetUserQuizResponses;

public class GetUserQuizResponsesEndpoint : Endpoint<GetUserQuizResponsesRequest, GetUserQuizResponsesResponse>
{
    private readonly IQuizRepository _quizRepository;
    private readonly IUserQuizResponseRepository _userResponseRepository;

    public GetUserQuizResponsesEndpoint(
        IQuizRepository quizRepository,
        IUserQuizResponseRepository userResponseRepository)
    {
        _quizRepository = quizRepository;
        _userResponseRepository = userResponseRepository;
    }

    public override void Configure()
    {
        Get("quiz/responses/{quizId}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetUserQuizResponsesRequest req, CancellationToken ct)
    {
        var userIdString = User.FindFirst("userId")?.Value;
        if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out var userId))
            throw new UnauthorizedAccessException("Invalid or missing user identifier");

        // Récupérer le quiz
        var quiz = _quizRepository.FindAll()
            .FirstOrDefault(q => q.Id == req.QuizId);

        if (quiz == null)
            throw new KeyNotFoundException($"Quiz with ID {req.QuizId} not found");

        // Récupérer les réponses de l'utilisateur
        var userResponses = await _userResponseRepository.GetByUserIdAndQuizAsync(userId, req.QuizId);

        // Mapper les réponses
        var responses = quiz.Questions
            .OrderBy(q => q.Order)
            .Select((question, index) =>
        {
            var ur = userResponses.FirstOrDefault(r => r.QuizQuestionId == question.Id);

            var dto = new QuestionResponseDto
            {
                QuestionNumber = index + 1,
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                QuestionType = question.QuestionType.ToString(),
                Placeholder = question.Placeholder
            };

            // Remplir selon le type de question
            if (question.QuestionType == Domain.Entities.QuizQuestionType.Scale1To10)
            {
                dto.ScaleMinLabel = question.ScaleMinLabel;
                dto.ScaleMidLabel = question.ScaleMidLabel;
                dto.ScaleMaxLabel = question.ScaleMaxLabel;

                // Créer les options d'échelle avec les labels
                dto.ScaleOptions = Enumerable.Range(1, 10).Select(i => 
                {
                    var label = question.ScaleLabels != null && question.ScaleLabels.Count >= i 
                        ? question.ScaleLabels[i - 1] 
                        : string.Empty;

                    return new ScaleOption
                    {
                        Value = i,
                        Label = label ?? string.Empty,
                        IsSelected = ur?.SelectedScore == i
                    };
                }).ToList();

                dto.SelectedScore = ur?.SelectedScore;
            }
            else if (question.QuestionType == Domain.Entities.QuizQuestionType.MultipleChoice)
            {
                // Créer les options de choix multiple
                dto.Options = question.Responses.Select(r => new MultipleChoiceOption
                {
                    Id = r.Id,
                    Text = r.ResponseText,
                    IsSelected = ur?.SelectedResponseId == r.Id
                }).ToList();

                dto.SelectedResponseId = ur?.SelectedResponseId;
                var selectedResponse = question.Responses.FirstOrDefault(r => r.Id == ur?.SelectedResponseId);
                dto.SelectedResponseText = selectedResponse?.ResponseText;
            }
            else if (question.QuestionType == Domain.Entities.QuizQuestionType.TextInput)
            {
                dto.SelectedTextResponse = ur?.SelectedTextResponse;
            }

            return dto;
        }).ToList();

        var response = new GetUserQuizResponsesResponse
        {
            QuizId = req.QuizId,
            QuizTitle = quiz.Titre ?? "Quiz",
            Responses = responses,
            TotalQuestions = quiz.Questions.Count,
            AnsweredQuestions = userResponses.Count,
            IsComplete = quiz.Questions.Count == userResponses.Count
        };

        await Send.OkAsync(response, cancellation: ct);
    }
}
