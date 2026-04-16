using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities;

public class EquipeMessage : AuditableAndSoftDeletableEntity
{
    public string? Texte { get; set; }
    public Guid ExpediteurId { get; set; }
    public DateTime Date { get; set; }
    public Guid EquipeConversationId { get; set; }
    public EquipeConversation EquipeConversation { get; set; } = null!;

    public string? AttachmentUrl { get; set; }
    public string? AttachmentFileName { get; set; }
    public string? AttachmentContentType { get; set; }

    public User Expediteur { get; private set; } = null!;
    public List<EquipeMessageRead> Reads { get; set; } = [];

    public void SanitazeForSaving()
    {
        Texte = Texte?.Trim();
    }
}
