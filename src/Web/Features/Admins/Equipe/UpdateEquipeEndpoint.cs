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
            Response = new SucceededOrNotResponse(false, new Error { ErrorType = "InvalidId", ErrorMessage = "ID invalide." });
            return;
        }

        var entity = await _repository.FindByIdWithMembersAndSousEquipes(guidId);

        if (entity is null)
        {
            Logger.LogWarning("[UpdateEquipe] Équipe introuvable: {Id}", guidId);
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(false, new Error { ErrorType = "NotFound", ErrorMessage = "Équipe introuvable." });
            return;
        }

        // Validation: parentEquipeId parseable
        Guid? parentEquipeId = null;
        if (!string.IsNullOrWhiteSpace(req.ParentEquipeId))
        {
            if (!Guid.TryParse(req.ParentEquipeId, out var parsedParentId))
            {
                HttpContext.Response.StatusCode = 400;
                Response = new SucceededOrNotResponse(false, new Error { ErrorType = "InvalidParentId", ErrorMessage = "L'identifiant de l'équipe parente est invalide." });
                return;
            }

            // Validation: une équipe ne peut pas être son propre parent
            if (parsedParentId == guidId)
            {
                HttpContext.Response.StatusCode = 400;
                Response = new SucceededOrNotResponse(false, new Error { ErrorType = "CircularReference", ErrorMessage = "Une équipe ne peut pas être son propre parent." });
                return;
            }

            // Validation: le parent ne peut pas être une sous-équipe de l'équipe courante (évite les cycles)
            var isCyclic = entity.SousEquipes.Any(s => s.Id == parsedParentId);
            if (isCyclic)
            {
                HttpContext.Response.StatusCode = 400;
                Response = new SucceededOrNotResponse(false, new Error { ErrorType = "CircularReference", ErrorMessage = "Le parent choisi est déjà une sous-équipe de cette équipe." });
                return;
            }

            // Validation: le parent doit exister et ne pas être soft-deleted
            var parentExists = await _repository.FindById(parsedParentId);
            if (parentExists is null)
            {
                HttpContext.Response.StatusCode = 400;
                Response = new SucceededOrNotResponse(false, new Error { ErrorType = "ParentNotFound", ErrorMessage = "L'équipe parente est introuvable ou supprimée." });
                return;
            }

            // Validation: le parent ne peut pas lui-même avoir un parent (max 2 niveaux)
            if (parentExists.ParentEquipeId.HasValue)
            {
                HttpContext.Response.StatusCode = 400;
                Response = new SucceededOrNotResponse(false, new Error { ErrorType = "MaxDepthExceeded", ErrorMessage = "La hiérarchie ne peut pas dépasser 2 niveaux (équipe → sous-équipe)." });
                return;
            }

            parentEquipeId = parsedParentId;
        }

        if (!string.IsNullOrWhiteSpace(req.NameFr))
            entity.NameFr = req.NameFr.Trim().CapitalizeFirstLetterOfEachWord()!;

        if (!string.IsNullOrWhiteSpace(req.NameEn))
            entity.NameEn = req.NameEn.Trim().CapitalizeFirstLetterOfEachWord()!;

        entity.SetParentEquipe(parentEquipeId);

        await _repository.UpdateEquipe(entity);

        var memberIds = (req.MemberIds ?? new List<string>())
            .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .ToList();

        await _repository.SetEquipeMembers(guidId, memberIds);

        Logger.LogInformation("[UpdateEquipe] Succès!");
        Response = new SucceededOrNotResponse(true);
    }
}
