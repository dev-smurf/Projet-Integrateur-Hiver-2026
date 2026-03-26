using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities;

public class AdminAvailabilityOverride : AuditableAndSoftDeletableEntity
{
    public Guid AdminId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public bool IsBlocked { get; set; }

    public User Admin { get; set; } = null!;
}
