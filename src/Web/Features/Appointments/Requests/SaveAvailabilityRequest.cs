namespace Web.Features.Appointments.Requests;

public class SaveAvailabilityRequest
{
    public List<AvailabilitySlot> Slots { get; set; } = [];
}

public class AvailabilitySlot
{
    public DayOfWeek DayOfWeek { get; set; }
    public string StartTime { get; set; } = null!; // "HH:mm"
    public string EndTime { get; set; } = null!;   // "HH:mm"
}
