using Domain.Common;

namespace Domain.Entities
{
    public class UserQuizResponse : AuditableAndSoftDeletableEntity
    {
        public Guid UserId { get; set; }
        public Guid QuizQuestionId { get; set; }

        public int? SelectedScore { get; set; }

        public Guid? SelectedResponseId { get; set; }

        public string? SelectedTextResponse { get; set; }

        public QuizQuestion Question { get; set; } = null!;
    }
}
