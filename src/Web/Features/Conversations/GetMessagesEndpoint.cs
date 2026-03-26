using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Conversations.Requests;

namespace Web.Features.Conversations;

public class GetMessagesEndpoint : Endpoint<GetMessagesRequest, object>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetMessagesEndpoint(
        IConversationRepository conversationRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _conversationRepository = conversationRepository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Get("conversations/{ConversationId}/messages");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetMessagesRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var userId = user.Id;

        var conversation = await _conversationRepository.GetByIdAsync(req.ConversationId);
        if (conversation == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (conversation.AdminId != userId && conversation.MemberId != userId)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var messages = await _conversationRepository.GetMessagesAsync(req.ConversationId, req.Page, req.PageSize);

        var result = messages.Select(m => new
        {
            m.Id,
            Text = m.Texte,
            SenderId = m.ExpediteurId,
            SenderName = m.Expediteur?.UserName,
            m.Date,
            m.ReadAt,
            m.ConversationId,
            m.AttachmentUrl,
            m.AttachmentFileName,
            m.AttachmentContentType,
            Type = (int)m.Type,
            m.AppointmentId,
            AppointmentDate = m.Appointment?.Date,
            AppointmentStatus = m.Appointment != null ? (int?)m.Appointment.Status : null,
            AppointmentMotif = m.Appointment?.Motif
        }).ToList();

        await Send.OkAsync(result, cancellation: ct);
    }
}
