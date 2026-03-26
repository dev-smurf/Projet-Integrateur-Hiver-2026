using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities;

public class AdminAvailability : AuditableAndSoftDeletableEntity
{
    public Guid AdminId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public User Admin { get; set; } = null!;
}
