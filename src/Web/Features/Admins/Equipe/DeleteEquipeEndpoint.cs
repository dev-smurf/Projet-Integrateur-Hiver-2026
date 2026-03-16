using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class DeleteEquipeEndpoint : EndpointWithoutRequest<SucceededOrNotResponse>
{
    private readonly IEquipeRepository _repository;

    public DeleteEquipeEndpoint(IEquipeRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/equipes/{id}");
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
            Response = new SucceededOrNotResponse(
                false,
                new Error { ErrorType = "InvalidId", ErrorMessage = "ID invalide." }
            );
            return;
        }

        var entity = await _repository.FindById(guidId);

        if (entity is null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(
                false,
                new Error { ErrorType = "NotFound", ErrorMessage = "Équipe introuvable." }
            );
            return;
        }

        entity.SoftDelete();

        await _repository.UpdateEquipe(entity);

        Response = new SucceededOrNotResponse(true);
    }
}
