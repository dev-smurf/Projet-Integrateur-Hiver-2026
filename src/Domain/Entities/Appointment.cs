using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities;

public enum AppointmentStatus
{
    Pending = 0,
    Accepted = 1,
    Refused = 2
}

public class Appointment : AuditableAndSoftDeletableEntity
{
    public Guid MemberId { get; set; }
    public Guid AdminId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);
    public string? Motif { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? RefusalReason { get; set; }
    public Guid ConversationId { get; set; }

    public User Member { get; set; } = null!;
    public User Admin { get; set; } = null!;
    public Conversation Conversation { get; set; } = null!;

    public void Accept()
    {
        Status = AppointmentStatus.Accepted;
    }

    public void Refuse(string? reason = null)
    {
        Status = AppointmentStatus.Refused;
        RefusalReason = reason?.Trim();
    }
}
