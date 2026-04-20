using Domain.Repositories;
using FastEndpoints;
using Application.Interfaces.Services.Equipe.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class GetEquipeByIdEndpoint : EndpointWithoutRequest<EquipeDto>
{
    private readonly IEquipeRepository _repository;

    public GetEquipeByIdEndpoint(IEquipeRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/equipe/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var idString = Route<string>("id");

        if (!Guid.TryParse(idString, out var guidId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = null!;
            return;
        }

        var entity = await _repository.FindByIdWithMembers(guidId);

        if (entity is null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = null!;
            return;
        }

        Response = new EquipeDto
        {
            Id = entity.Id.ToString(),
            NameFr = entity.NameFr,
            NameEn = entity.NameEn,
            MemberUserIds = entity.Membres.Select(u => u.Id.ToString()).ToList()
        };
    }
}
