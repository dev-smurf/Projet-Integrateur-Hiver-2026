using Domain.Entities;

namespace Web.Features.Admins.Quiz.CreateQuestion;

public class CreateQuestionRequest : Web.Features.Common.ISanitizable
{
    public Guid QuizId { get; set; }
    public string QuestionText { get; set; } = null!;
    public int Order { get; set; }
    public QuizQuestionType QuestionType { get; set; }
    public string? Placeholder { get; set; }
    public List<CreateResponseRequest> Responses { get; set; } = new();

    public void Sanitize()
    {
        QuestionText = QuestionText?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(Placeholder))
            Placeholder = Placeholder.Trim();

        foreach (var response in Responses)
        {
            response.Sanitize();
        }
    }
}

public class CreateResponseRequest
{
    public string ResponseText { get; set; } = null!;
    public int Order { get; set; }

    public void Sanitize()
    {
        ResponseText = ResponseText?.Trim() ?? string.Empty;
    }
}
