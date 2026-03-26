using Domain.Common;
using Domain.Extensions;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Equipe;

public class UpdateEquipeEndpoint : Endpoint<EditEquipeRequest, SucceededOrNotResponse>
{
    private readonly IEquipeRepository _repository;

    public UpdateEquipeEndpoint(IEquipeRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/equipe/{id}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(EditEquipeRequest req, CancellationToken ct)
    {
        var idString = Route<string>("id");

        if (!Guid.TryParse(idString, out var guidId))
        {
            Logger.LogWarning("[UpdateEquipe] GUID invalide: {Id}", idString);
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
            Logger.LogWarning("[UpdateEquipe] Équipe introuvable: {Id}", guidId);
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(
                false,
                new Error { ErrorType = "NotFound", ErrorMessage = "Équipe introuvable." }
            );
            return;
        }

        Logger.LogInformation("[UpdateEquipe] Équipe trouvée, mise à jour...");

        if (!string.IsNullOrWhiteSpace(req.NameFr) || !string.IsNullOrWhiteSpace(req.NameEn))
        {
            entity.NameFr = req.NameFr.Trim().CapitalizeFirstLetterOfEachWord()!;
            entity.NameEn = req.NameEn.Trim().CapitalizeFirstLetterOfEachWord()!;
        }

        Logger.LogInformation("[UpdateEquipe] Mise à jour en base de données...");
        await _repository.UpdateEquipe(entity);

        Logger.LogInformation("[UpdateEquipe] Succès!");
        Response = new SucceededOrNotResponse(true);
    }

}
