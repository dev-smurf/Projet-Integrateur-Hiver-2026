using Domain.Common;

namespace Domain.Entities
{
    public class QuizAssignment : AuditableAndSoftDeletableEntity
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public int Version { get; set; } = 1;
        public string? FollowUpLabel { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? AvailableAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Quiz Quiz { get; set; } = null!;
        public ICollection<UserQuizResponse> Responses { get; set; } = new List<UserQuizResponse>();
    }
}
