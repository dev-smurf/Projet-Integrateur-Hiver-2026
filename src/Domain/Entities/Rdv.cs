using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Rdv : AuditableAndSoftDeletableEntity
    {
        public string Titre { get; private set; } = null!;
        public DateTime Date { get; private set; }
        public TimeSpan Duree { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public string Note { get; private set; } = null!;

        private Rdv() { }

        public Rdv(string titre, DateTime date, TimeSpan duree, Guid userId, string note)
        {
            Titre = titre;
            Date = date;
            Duree = duree;
            UserId = userId;
            Note = note;
        }

        public void SanitazeForSaving()
        {
            Titre = Titre.Trim();
            Note = Note.Trim();
        }
    }
}
