using Application.Interfaces.Services.Users;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Web.Features.Appointments.Requests;
using Web.Hubs;

namespace Web.Features.Appointments;

public class RequestAppointmentEndpoint : Endpoint<RequestAppointmentRequest, object>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IAdminAvailabilityRepository _availabilityRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IHubContext<ChatHub> _hubContext;

    public RequestAppointmentEndpoint(
        IAppointmentRepository appointmentRepository,
        IConversationRepository conversationRepository,
        IAdminAvailabilityRepository availabilityRepository,
        IAuthenticatedUserService authenticatedUserService,
        IHubContext<ChatHub> hubContext)
    {
        _appointmentRepository = appointmentRepository;
        _conversationRepository = conversationRepository;
        _availabilityRepository = availabilityRepository;
        _authenticatedUserService = authenticatedUserService;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Post("appointments/request");
        Roles(Domain.Constants.User.Roles.MEMBER);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(RequestAppointmentRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        // Ensure conversation exists for this member
        var conversation = await _conversationRepository.EnsureConversationForMemberAsync(user.Id);
        if (conversation == null)
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var adminId = conversation.AdminId;

        // Check for time conflict
        if (await _appointmentRepository.HasConflictAsync(adminId, req.Date))
        {
            HttpContext.Response.StatusCode = 409;
            return;
        }

        // Create the appointment
        var appointment = new Appointment
        {
            MemberId = user.Id,
            AdminId = adminId,
            Date = req.Date,
            Duration = TimeSpan.FromHours(1),
            Motif = req.Motif?.Trim(),
            Status = AppointmentStatus.Pending,
            ConversationId = conversation.Id
        };

        await _appointmentRepository.CreateAsync(appointment);

        // Create a special chat message for the appointment request
        var message = new Message
        {
            Texte = req.Motif?.Trim(),
            ExpediteurId = user.Id,
            ReceveurId = adminId,
            Date = DateTime.UtcNow,
            ConversationId = conversation.Id,
            Type = MessageType.AppointmentRequest,
            AppointmentId = appointment.Id
        };

        var saved = await _conversationRepository.AddMessageAsync(message);
        await _conversationRepository.UpdateLastMessageAtAsync(conversation.Id, saved.Date);

        var payload = new
        {
            saved.Id,
            Text = saved.Texte,
            SenderId = saved.ExpediteurId,
            saved.Date,
            saved.ConversationId,
            Type = (int)saved.Type,
            saved.AppointmentId,
            AppointmentDate = appointment.Date,
            AppointmentStatus = (int)appointment.Status,
            AppointmentMotif = appointment.Motif,
            AttachmentUrl = (string?)null,
            AttachmentFileName = (string?)null,
            AttachmentContentType = (string?)null
        };

        await _hubContext.Clients.Group($"user_{adminId}").SendAsync("ReceiveMessage", payload, ct);

        await Send.OkAsync(payload, cancellation: ct);
    }
}
