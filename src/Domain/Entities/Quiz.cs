using Domain.Common;

namespace Domain.Entities
{
    public class Quiz : AuditableAndSoftDeletableEntity
    {
        public string Titre { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // Navigation property
        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();

        public void SanitazeForSaving()
        {
            Titre = Titre?.Trim() ?? string.Empty;
            Description = Description?.Trim();
            ImageUrl = ImageUrl?.Trim();
        }
    }
}
