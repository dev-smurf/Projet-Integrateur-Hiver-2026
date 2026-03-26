using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities;

public class Conversation : AuditableAndSoftDeletableEntity
{
    public Guid AdminId { get; set; }
    public Guid MemberId { get; set; }
    public DateTime LastMessageAt { get; set; }

    public User Admin { get; set; } = null!;
    public User Member { get; set; } = null!;
    public List<Message> Messages { get; set; } = [];
}
