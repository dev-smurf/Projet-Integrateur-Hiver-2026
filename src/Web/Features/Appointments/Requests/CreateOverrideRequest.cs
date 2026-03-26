namespace Web.Features.Appointments.Requests;

public class CreateOverrideRequest
{
    public DateTime Date { get; set; }
    public string? StartTime { get; set; } // "HH:mm", null if blocked
    public string? EndTime { get; set; }   // "HH:mm", null if blocked
    public bool IsBlocked { get; set; }
}
