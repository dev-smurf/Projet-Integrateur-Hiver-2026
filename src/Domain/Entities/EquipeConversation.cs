using Domain.Common;

namespace Domain.Entities;

public class EquipeConversation : AuditableAndSoftDeletableEntity
{
    public Guid EquipeId { get; set; }
    public DateTime LastMessageAt { get; set; }

    public Equipe Equipe { get; set; } = null!;
    public List<EquipeMessage> Messages { get; set; } = [];
}
