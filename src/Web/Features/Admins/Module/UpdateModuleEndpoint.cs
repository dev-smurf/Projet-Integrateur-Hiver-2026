using Domain.Common;
using Domain.Repositories;
using FastEndpoints;

namespace Web.Features.Admins.Module;

public class UpdateModuleEndpoint : Endpoint<EditModuleRequest, SucceededOrNotResponse>
{
    private readonly IModuleRepository _repository;

    public UpdateModuleEndpoint(IModuleRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/module/{id}");
        AllowFileUploads();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EditModuleRequest req, CancellationToken ct)
    {
        var idString = Route<string>("id");

        Logger.LogInformation("[UpdateModule] Début - ID: {Id}", idString);
        Logger.LogInformation("[UpdateModule] Request - NameFr: {NameFr}, SujetFr: {SujetFr}", req.NameFr, req.SujetFr);

        if (!Guid.TryParse(idString, out var guidId))
        {
            Logger.LogWarning("[UpdateModule] GUID invalide: {Id}", idString);
            HttpContext.Response.StatusCode = 400;
            Response = new SucceededOrNotResponse(
                false,
                new Error { ErrorType = "InvalidId", ErrorMessage = "ID invalide." }
            );
            return;
        }

        var entity = await _repository.GetByIdAsync(guidId);

        if (entity is null)
        {
            Logger.LogWarning("[UpdateModule] Module introuvable: {Id}", guidId);
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(
                false,
                new Error { ErrorType = "NotFound", ErrorMessage = "Module introuvable." }
            );
            return;
        }

        Logger.LogInformation("[UpdateModule] Module trouvé, mise à jour...");

        if (!string.IsNullOrWhiteSpace(req.NameFr))
            entity.NameFr = req.NameFr;

        if (!string.IsNullOrWhiteSpace(req.NameEn))
            entity.NameEn = req.NameEn;

        if (!string.IsNullOrWhiteSpace(req.SujetFr))
            entity.SujetFr = req.SujetFr;

        if (!string.IsNullOrWhiteSpace(req.SujetEn))
            entity.SujetEn = req.SujetEn;

        if (!string.IsNullOrWhiteSpace(req.ContenueFr))
            entity.ContenueFr = req.ContenueFr;

        if (!string.IsNullOrWhiteSpace(req.ContenueEn))
            entity.ContenueEn = req.ContenueEn;

        if (req.CardImage is not null)
        {
            Logger.LogInformation("[UpdateModule] Sauvegarde de l'image...");
            entity.CardImageUrl = await SaveFile(req.CardImage);
        }

        entity.SanitizeForSaving();

        Logger.LogInformation("[UpdateModule] Mise à jour en base de données...");
        await _repository.UpdateAsync(entity);

        Logger.LogInformation("[UpdateModule] Succès!");
        Response = new SucceededOrNotResponse(true);
    }

    private static async Task<string> SaveFile(IFormFile file, string folder = "uploads")
    {
        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        var fileExtension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPath = Path.Combine(savePath, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/{folder}/{fileName}";
    }
}
