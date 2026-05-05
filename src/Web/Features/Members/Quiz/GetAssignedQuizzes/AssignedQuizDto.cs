namespace Web.Features.Members.Quiz.GetAssignedQuizzes;

public class AssignedQuizDto
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public int Version { get; set; }
    public string? FollowUpLabel { get; set; }
    public string Titre { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? AvailableAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted => CompletedAt.HasValue;
}
