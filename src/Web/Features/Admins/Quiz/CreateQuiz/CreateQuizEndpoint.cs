using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IMapper = AutoMapper.IMapper;

namespace Web.Features.Admins.Quiz.CreateQuiz;

public class CreateQuizEndpoint : Endpoint<CreateQuizRequest, EmptyResponse>
{
    private readonly IMapper _mapper;
    private readonly IQuizRepository _quizRepository;

    public CreateQuizEndpoint(
        IMapper mapper,
        IQuizRepository quizRepository)
    {
        _mapper = mapper;
        _quizRepository = quizRepository;
    }

    public override void Configure()
    {
        DontCatchExceptions();
        Post("quiz");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CreateQuizRequest req, CancellationToken ct)
    {
        try
        {
            System.Console.WriteLine($"[CreateQuizEndpoint] Creating quiz: {req.Titre}");
            System.Console.WriteLine($"[CreateQuizEndpoint] Questions count: {req.Questions?.Count ?? 0}");

            // Create quiz entity
            var quiz = new Domain.Entities.Quiz
            {
                Titre = req.Titre,
                Description = req.Description,
                ImageUrl = req.ImageUrl
            };

            // Create questions and responses
            foreach (var questionReq in req.Questions)
            {
                var question = new QuizQuestion
                {
                    QuestionText = questionReq.QuestionText,
                    Order = questionReq.Order,
                    QuestionType = questionReq.QuestionType,
                    Placeholder = questionReq.Placeholder,
                    ScaleMinLabel = questionReq.ScaleMinLabel ?? "Jamais",
                    ScaleMidLabel = questionReq.ScaleMidLabel ?? "Parfois",
                    ScaleMaxLabel = questionReq.ScaleMaxLabel ?? "Toujours",
                    ScaleLabels = (questionReq.ScaleLabels != null && questionReq.ScaleLabels.Count > 0)
                        ? questionReq.ScaleLabels
                        : Enumerable.Repeat(string.Empty, 10).ToList()
                };

                foreach (var responseReq in questionReq.Responses)
                {
                    var response = new QuizQuestionResponse
                    {
                        ResponseText = responseReq.ResponseText,
                        Order = responseReq.Order
                    };
                    question.Responses.Add(response);
                }

                quiz.Questions.Add(question);
            }

            quiz.SanitazeForSaving();
            System.Console.WriteLine($"[CreateQuizEndpoint] Saving quiz with ID: {quiz.Id}");
            await _quizRepository.Create(quiz);
            System.Console.WriteLine($"[CreateQuizEndpoint] Quiz saved successfully!");

            await Send.NoContentAsync(ct);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"[CreateQuizEndpoint] ERROR: {ex.Message}");
            System.Console.WriteLine($"[CreateQuizEndpoint] STACK: {ex.StackTrace}");
            throw;
        }
    }
}

