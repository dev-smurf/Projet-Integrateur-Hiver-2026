using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Web.Features.Conversations.Requests;
using Web.Hubs;

namespace Web.Features.Conversations;

public class MarkAsReadEndpoint : Endpoint<MarkAsReadRequest, object>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;
    private readonly IHubContext<ChatHub> _hubContext;

    public MarkAsReadEndpoint(
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
        Put("conversations/{ConversationId}/read");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(MarkAsReadRequest req, CancellationToken ct)
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

        await _conversationRepository.MarkMessagesAsReadAsync(req.ConversationId, userId);

        var otherUserId = conversation.AdminId == userId ? conversation.MemberId : conversation.AdminId;
        await _hubContext.Clients.Group($"user_{otherUserId}").SendAsync("MessageRead", new { req.ConversationId }, ct);

        await Send.OkAsync(new { success = true }, cancellation: ct);
    }
}
