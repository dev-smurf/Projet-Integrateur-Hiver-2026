using System.Security.Claims;
using Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private readonly IEquipeConversationRepository _equipeConversationRepository;

    public ChatHub(IEquipeConversationRepository equipeConversationRepository)
    {
        _equipeConversationRepository = equipeConversationRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var userIdStr = Context.User?.FindFirst("userId")?.Value;
        if (userIdStr != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userIdStr}");

            if (Guid.TryParse(userIdStr, out var userId))
            {
                var isAdmin = Context.User?.IsInRole(Domain.Constants.User.Roles.ADMINISTRATOR) ?? false;
                var equipeIds = await _equipeConversationRepository.GetEquipeIdsForUserAsync(userId, isAdmin);
                foreach (var equipeId in equipeIds)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"equipe_{equipeId}");
                }
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdStr = Context.User?.FindFirst("userId")?.Value;
        if (userIdStr != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userIdStr}");

            if (Guid.TryParse(userIdStr, out var userId))
            {
                var isAdmin = Context.User?.IsInRole(Domain.Constants.User.Roles.ADMINISTRATOR) ?? false;
                var equipeIds = await _equipeConversationRepository.GetEquipeIdsForUserAsync(userId, isAdmin);
                foreach (var equipeId in equipeIds)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"equipe_{equipeId}");
                }
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendTyping(string conversationId, string recipientUserId)
    {
        var senderId = Context.User?.FindFirst("userId")?.Value;
        if (senderId != null)
        {
            await Clients.Group($"user_{recipientUserId}").SendAsync("UserTyping", new
            {
                ConversationId = conversationId,
                SenderId = senderId
            });
        }
    }

    public async Task SendEquipeTyping(string conversationId, string equipeId)
    {
        var senderId = Context.User?.FindFirst("userId")?.Value;
        if (senderId != null)
        {
            await Clients.GroupExcept($"equipe_{equipeId}", Context.ConnectionId).SendAsync("EquipeUserTyping", new
            {
                ConversationId = conversationId,
                EquipeId = equipeId,
                SenderId = senderId
            });
        }
    }
}
