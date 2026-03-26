namespace Web.Features.Appointments.Requests;

public class RespondAppointmentRequest
{
    public Guid AppointmentId { get; set; }
    public bool Accept { get; set; }
    public string? Reason { get; set; }
}
