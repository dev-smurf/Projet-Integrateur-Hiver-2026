using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public enum MessageType
    {
        Text = 0,
        AppointmentRequest = 1,
        AppointmentResponse = 2
    }

    public class Message : AuditableAndSoftDeletableEntity
    {
        public string? Texte { get; set; }
        public Guid ExpediteurId { get; set; }
        public Guid ReceveurId { get; set; }
        public DateTime Date { get; set; }
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;
        public DateTime? ReadAt { get; private set; }

        public string? AttachmentUrl { get; set; }
        public string? AttachmentFileName { get; set; }
        public string? AttachmentContentType { get; set; }

        public MessageType Type { get; set; } = MessageType.Text;
        public Guid? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public User Expediteur { get; private set; } = null!;
        public User Receveur { get; private set; } = null!;

        public void MarkAsRead()
        {
            ReadAt = DateTime.UtcNow;
        }

        public void SanitazeForSaving()
        {
            Texte = Texte?.Trim();
        }
    }
}
