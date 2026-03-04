using Application.Interfaces.Services.Users;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Web.Features.Conversations.Requests;

namespace Web.Features.Conversations;

public class CreateConversationEndpoint : Endpoint<CreateConversationRequest, object>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public CreateConversationEndpoint(
        IConversationRepository conversationRepository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _conversationRepository = conversationRepository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Post("conversations");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CreateConversationRequest req, CancellationToken ct)
    {
        var user = _authenticatedUserService.GetAuthenticatedUser();
        if (user == null)
        {
            HttpContext.Response.StatusCode = 401;
            return;
        }

        var adminId = user.Id;

        var existing = await _conversationRepository.GetByAdminAndMemberAsync(adminId, req.MemberId);
        if (existing != null)
        {
            await Send.OkAsync(new { id = existing.Id }, cancellation: ct);
            return;
        }

        var conversation = new Conversation
        {
            AdminId = adminId,
            MemberId = req.MemberId,
            LastMessageAt = DateTime.UtcNow
        };

        var created = await _conversationRepository.CreateAsync(conversation);
        await Send.OkAsync(new { id = created.Id }, cancellation: ct);
    }
}
