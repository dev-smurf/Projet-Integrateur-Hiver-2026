using Domain.Entities;

namespace Web.Features.Members.Quiz.SubmitQuiz;

public class SubmitQuizRequest
{
    public Guid QuizAssignmentId { get; set; }
    public Guid QuizQuestionId { get; set; }
    public int? SelectedScore { get; set; }
    public Guid? SelectedResponseId { get; set; }
    public List<Guid>? SelectedResponseIds { get; set; }
    public string? SelectedTextResponse { get; set; }
}

public class SubmitQuizResponse
{
    public Guid QuizQuestionId { get; set; }
    public QuizQuestionType QuestionType { get; set; }
    public string QuestionText { get; set; } = null!;
    public bool IsValid { get; set; }
    public string? Message { get; set; }
}
