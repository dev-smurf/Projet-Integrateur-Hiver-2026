using Domain.Entities.Identity;

namespace Domain.Entities;

public class EquipeMessageRead
{
    public Guid EquipeMessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReadAt { get; set; }

    public EquipeMessage EquipeMessage { get; set; } = null!;
    public User User { get; set; } = null!;
}
