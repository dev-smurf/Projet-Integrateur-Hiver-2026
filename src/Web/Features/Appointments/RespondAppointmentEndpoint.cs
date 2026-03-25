using Application.Interfaces.Services.Users;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Web.Features.Appointments.Requests;
using Web.Hubs;

namespace Web.Features.Appointments;

public class RespondAppointmentEndpoint : Endpoint<RespondAppointmentRequest, object>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IHubContext<ChatHub> _hubContext;

    public RespondAppointmentEndpoint(
        IAppointmentRepository appointmentRepository,
        IConversationRepository conversationRepository,
        IAuthenticatedUserService authenticatedUserService,
        IHubContext<ChatHub> hubContext)
    {
        _appointmentRepository = appointmentRepository;
        _conversationRepository = conversationRepository;
        _authenticatedUserService = authenticatedUserService;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Post("appointments/respond");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(RespondAppointmentRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var appointment = await _appointmentRepository.GetByIdAsync(req.AppointmentId);
        if (appointment == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (appointment.AdminId != user.Id)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        if (appointment.Status != AppointmentStatus.Pending)
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        if (req.Accept)
            appointment.Accept();
        else
            appointment.Refuse(req.Reason);

        await _appointmentRepository.UpdateAsync(appointment);

        // Create a response message in chat
        var message = new Message
        {
            Texte = req.Accept ? null : req.Reason?.Trim(),
            ExpediteurId = user.Id,
            ReceveurId = appointment.MemberId,
            Date = DateTime.UtcNow,
            ConversationId = appointment.ConversationId,
            Type = MessageType.AppointmentResponse,
            AppointmentId = appointment.Id
        };

        var saved = await _conversationRepository.AddMessageAsync(message);
        await _conversationRepository.UpdateLastMessageAtAsync(appointment.ConversationId, saved.Date);

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

        await _hubContext.Clients.Group($"user_{appointment.MemberId}").SendAsync("ReceiveMessage", payload, ct);

        await Send.OkAsync(payload, cancellation: ct);
    }
}
