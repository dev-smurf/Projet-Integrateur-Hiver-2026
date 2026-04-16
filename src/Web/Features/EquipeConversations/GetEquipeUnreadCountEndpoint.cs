using Application.Interfaces.Services.Users;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.EquipeConversations;

public class GetEquipeUnreadCountEndpoint : EndpointWithoutRequest<object>
{
    private readonly IEquipeConversationRepository _repository;
    private readonly IAuthenticatedUserService _authenticatedUserService;

    public GetEquipeUnreadCountEndpoint(
        IEquipeConversationRepository repository,
        IAuthenticatedUserService authenticatedUserService)
    {
        _repository = repository;
        _authenticatedUserService = authenticatedUserService;
    }

    public override void Configure()
    {
        Get("equipe-conversations/unread-count");
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

        var count = await _repository.GetUnreadCountAsync(user.Id);
        await Send.OkAsync(new { count }, cancellation: ct);
    }
}
