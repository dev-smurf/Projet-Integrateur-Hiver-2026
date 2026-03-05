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

        // Auto-create conversations for all members
        if (isAdmin)
            await _conversationRepository.EnsureConversationsForAdminAsync(userId);
        else
            await _conversationRepository.EnsureConversationForMemberAsync(userId);

        var conversations = (await _conversationRepository.GetByUserIdAsync(userId, isAdmin)).ToList();
        var unreadCounts = await _conversationRepository.GetUnreadCountsPerConversationAsync(userId);

        var memberIds = conversations.Select(c => c.MemberId).Distinct();
        var adminIds = conversations.Select(c => c.AdminId).Distinct();
        var memberNames = await _conversationRepository.GetMemberNamesByUserIdsAsync(memberIds);
        var adminNames = await _conversationRepository.GetAdminNamesByUserIdsAsync(adminIds);

        var result = conversations.Select(c => new
        {
            c.Id,
            MemberName = memberNames.TryGetValue(c.MemberId, out var mn) ? mn : "Unknown",
            AdminName = adminNames.TryGetValue(c.AdminId, out var an) ? an : "Admin",
            c.MemberId,
            c.AdminId,
            LastMessage = c.Messages.OrderByDescending(m => m.Date).FirstOrDefault() is { } lastMsg
                ? lastMsg.Texte ?? lastMsg.AttachmentFileName
                : null,
            c.LastMessageAt,
            UnreadCount = unreadCounts.TryGetValue(c.Id, out var count) ? count : 0
        }).ToList();

        await Send.OkAsync(result, cancellation: ct);
    }
}
