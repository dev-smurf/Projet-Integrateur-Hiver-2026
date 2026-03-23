using Domain.Entities;

namespace Web.Features.Admins.Quiz.UpdateQuestion;

public class UpdateQuestionRequest : Web.Features.Common.ISanitizable
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = null!;
    public int Order { get; set; }
    public QuizQuestionType QuestionType { get; set; }
    public string? Placeholder { get; set; }
    public List<UpdateResponseRequest> Responses { get; set; } = new();

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

public class UpdateResponseRequest
{
    public Guid? Id { get; set; }
    public string ResponseText { get; set; } = null!;
    public int Order { get; set; }

    public void Sanitize()
    {
        ResponseText = ResponseText?.Trim() ?? string.Empty;
    }
}
