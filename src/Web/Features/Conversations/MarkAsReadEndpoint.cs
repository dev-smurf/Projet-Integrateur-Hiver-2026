using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Conversations.Requests;

namespace Web.Features.Conversations;

public class MarkAsReadEndpoint : Endpoint<MarkAsReadRequest, object>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public MarkAsReadEndpoint(
        IConversationRepository conversationRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _conversationRepository = conversationRepository;
        _authenticatedUserService = authenticatedUserService;
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
        await Send.OkAsync(new { success = true }, cancellation: ct);
    }
}
