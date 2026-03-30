using Domain.Common;

namespace Domain.Entities
{
    public class QuizQuestionResponse : AuditableAndSoftDeletableEntity
    {
        public Guid QuizQuestionId { get; set; }
        public string ResponseText { get; set; } = null!;
        public int Order { get; set; }

        public QuizQuestion Question { get; set; } = null!;

        public void SanitazeForSaving()
        {
            ResponseText = ResponseText?.Trim() ?? string.Empty;
        }
    }
}
