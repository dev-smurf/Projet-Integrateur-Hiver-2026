using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.EquipeConversations.Requests;

namespace Web.Features.EquipeConversations;

public class MarkEquipeAsReadEndpoint : Endpoint<MarkEquipeAsReadRequest, object>
{
    private readonly IEquipeConversationRepository _repository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public MarkEquipeAsReadEndpoint(
        IEquipeConversationRepository repository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _repository = repository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Put("equipe-conversations/{ConversationId}/read");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(MarkEquipeAsReadRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var userId = user.Id;
        var isAdmin = User.IsInRole(Domain.Constants.User.Roles.ADMINISTRATOR);

        var conversation = await _repository.GetByIdAsync(req.ConversationId);
        if (conversation == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var authorized = await _repository.IsUserInEquipeAsync(userId, conversation.EquipeId, isAdmin);
        if (!authorized)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        await _repository.MarkAsReadAsync(req.ConversationId, userId);

        await Send.OkAsync(new { success = true }, cancellation: ct);
    }
}
