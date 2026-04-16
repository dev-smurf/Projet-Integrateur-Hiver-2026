using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.EquipeConversations;

public class GetEquipeConversationsEndpoint : EndpointWithoutRequest<object>
{
    private readonly IEquipeConversationRepository _repository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetEquipeConversationsEndpoint(
        IEquipeConversationRepository repository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _repository = repository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Get("equipe-conversations");
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

        if (isAdmin)
            await _repository.EnsureConversationsForAdminAsync();

        var conversations = (await _repository.GetByUserIdAsync(userId, isAdmin)).ToList();
        var unreadCounts = await _repository.GetUnreadCountsPerConversationAsync(userId);

        var result = conversations.Select(c => new
        {
            c.Id,
            EquipeId = c.EquipeId,
            EquipeName = c.Equipe.NameFr,
            EquipeNameFr = c.Equipe.NameFr,
            EquipeNameEn = c.Equipe.NameEn,
            MembersCount = c.Equipe.Membres?.Count ?? 0,
            LastMessage = c.Messages.OrderByDescending(m => m.Date).FirstOrDefault() is { } lastMsg
                ? lastMsg.Texte ?? lastMsg.AttachmentFileName
                : null,
            c.LastMessageAt,
            UnreadCount = unreadCounts.TryGetValue(c.Id, out var count) ? count : 0
        }).ToList();

        await Send.OkAsync(result, cancellation: ct);
    }
}
