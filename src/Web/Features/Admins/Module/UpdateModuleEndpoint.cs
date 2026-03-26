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

        if (!Guid.TryParse(idString, out var guidId))
        {
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
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(
                false,
                new Error { ErrorType = "NotFound", ErrorMessage = "Module introuvable." }
            );
            return;
        }

        if (!string.IsNullOrWhiteSpace(req.Name))
            entity.Name = req.Name;

        if (!string.IsNullOrWhiteSpace(req.Subject))
            entity.Subject = req.Subject;

        if (!string.IsNullOrWhiteSpace(req.Content))
            entity.Content = req.Content;

        if (req.CardImage is not null)
        {
            entity.CardImageUrl = await SaveFile(req.CardImage);
        }

        await _repository.UpdateAsync(entity);

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
