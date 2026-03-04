using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Conversations;

public class GetConversationsEndpoint : EndpointWithoutRequest<object>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetConversationsEndpoint(
        IConversationRepository conversationRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _conversationRepository = conversationRepository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Get("conversations");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var userId = user.Id;
        var isAdmin = User.IsInRole(Domain.Constants.User.Roles.ADMINISTRATOR);

        var conversations = await _conversationRepository.GetByUserIdAsync(userId, isAdmin);
        var unreadCounts = await _conversationRepository.GetUnreadCountsPerConversationAsync(userId);

        var result = conversations.Select(c => new
        {
            c.Id,
            MemberName = c.Member?.UserName,
            AdminName = c.Admin?.UserName,
            c.MemberId,
            c.AdminId,
            LastMessage = c.Messages.OrderByDescending(m => m.Date).FirstOrDefault()?.Texte,
            c.LastMessageAt,
            UnreadCount = unreadCounts.TryGetValue(c.Id, out var count) ? count : 0
        });

        await Send.OkAsync(result, cancellation: ct);
    }
}
