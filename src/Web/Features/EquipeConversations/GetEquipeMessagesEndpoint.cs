using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.EquipeConversations.Requests;

namespace Web.Features.EquipeConversations;

public class GetEquipeMessagesEndpoint : Endpoint<GetEquipeMessagesRequest, object>
{
    private readonly IEquipeConversationRepository _repository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetEquipeMessagesEndpoint(
        IEquipeConversationRepository repository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _repository = repository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Get("equipe-conversations/{ConversationId}/messages");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(GetEquipeMessagesRequest req, CancellationToken ct)
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

        var messages = (await _repository.GetMessagesAsync(req.ConversationId, req.Page, req.PageSize)).ToList();

        var senderIds = messages.Select(m => m.ExpediteurId).Distinct();
        var senderNames = await _repository.GetSenderNamesByUserIdsAsync(senderIds);

        var result = messages.Select(m => new
        {
            m.Id,
            Text = m.Texte,
            SenderId = m.ExpediteurId,
            SenderName = senderNames.TryGetValue(m.ExpediteurId, out var n) ? n : (m.Expediteur?.UserName ?? "User"),
            m.Date,
            EquipeConversationId = m.EquipeConversationId,
            m.AttachmentUrl,
            m.AttachmentFileName,
            m.AttachmentContentType
        }).ToList();

        await Send.OkAsync(result, cancellation: ct);
    }
}
