namespace Web.Features.Admins.Quiz.AssignQuiz;

public class AssignQuizRequest
{
    public Guid QuizId { get; set; }
    public List<Guid> UserIds { get; set; } = new();
    public DateTime? DueDate { get; set; }
}
