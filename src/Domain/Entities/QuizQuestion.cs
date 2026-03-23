using Domain.Common;

namespace Domain.Entities
{
    public class QuizQuestion : AuditableAndSoftDeletableEntity
    {
        public Guid QuizId { get; set; }
        public string QuestionText { get; set; } = null!;
        public int Order { get; set; }
        public QuizQuestionType QuestionType { get; set; } = QuizQuestionType.Scale1To10;
        public string? Placeholder { get; set; }

        public Quiz Quiz { get; set; } = null!;

        public ICollection<QuizQuestionResponse> Responses { get; set; } = new List<QuizQuestionResponse>();

        public void SanitazeForSaving()
        {
            QuestionText = QuestionText?.Trim() ?? string.Empty;
            Placeholder = Placeholder?.Trim();
        }
    }
}
