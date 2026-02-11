using Domain.Common;

namespace Domain.Entities
{
    public class Quiz: AuditableAndSoftDeletableEntity
    {
        public string Question { get; private set; } = null!;
        public string Reponse { get; private set; } = null!;
        public string Titre { get; private set; } = null!;
        public void SanitazeForSaving()
        {
                Question = Question.Trim();
                Reponse = Reponse.Trim();
                Titre = Titre.Trim();
        }
    }
}
