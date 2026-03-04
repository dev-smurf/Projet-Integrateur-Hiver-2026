using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Message : AuditableAndSoftDeletableEntity
    {
        public string Texte { get; private set; } = null!;
        public Guid ExpediteurId { get; private set; }
        public Guid ReceveurId { get; private set; }
        public DateTime Date { get; private set; }
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;
        public DateTime? ReadAt { get; private set; }

        public User Expediteur { get; private set; } = null!;
        public User Receveur { get; private set; } = null!;

        public void MarkAsRead()
        {
            ReadAt = DateTime.UtcNow;
        }

        public void SanitazeForSaving()
        {
            Texte = Texte.Trim();
        }
    }
}
