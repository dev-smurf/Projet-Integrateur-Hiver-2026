using Application.Interfaces.Services.Users;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Web.Features.Conversations.Requests;
using Web.Hubs;

namespace Web.Features.Conversations;

public class SendMessageEndpoint : Endpoint<SendMessageRequest, object>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IHubContext<ChatHub> _hubContext;

    public SendMessageEndpoint(
        IConversationRepository conversationRepository,
        IAuthenticatedUserService authenticatedUserService,
        IHubContext<ChatHub> hubContext)
    {
        _conversationRepository = conversationRepository;
        _authenticatedUserService = authenticatedUserService;
        _hubContext = hubContext;
    }

    public override void Configure()
    {
        Post("conversations/{ConversationId}/messages");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(SendMessageRequest req, CancellationToken ct)
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

        var receiverId = conversation.AdminId == userId
            ? conversation.MemberId
            : conversation.AdminId;

        var message = new Message
        {
            Texte = req.Text,
            ExpediteurId = userId,
            ReceveurId = receiverId,
            Date = DateTime.UtcNow,
            ConversationId = req.ConversationId
        };

        var saved = await _conversationRepository.AddMessageAsync(message);
        await _conversationRepository.UpdateLastMessageAtAsync(req.ConversationId, saved.Date);

        var payload = new
        {
            saved.Id,
            Text = saved.Texte,
            SenderId = saved.ExpediteurId,
            saved.Date,
            saved.ConversationId
        };

        await _hubContext.Clients.Group($"user_{receiverId}").SendAsync("ReceiveMessage", payload, ct);

        await Send.OkAsync(payload, cancellation: ct);
    }
}
