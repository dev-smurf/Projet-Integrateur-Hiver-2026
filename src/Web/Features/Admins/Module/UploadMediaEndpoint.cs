using Domain.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Module;

public class UploadMediaRequest
{
    public IFormFile File { get; set; } = null!;
}

public class UploadMediaResponse
{
    public string Url { get; set; } = null!;
}

public class UploadMediaEndpoint : Endpoint<UploadMediaRequest, UploadMediaResponse>
{
    private static readonly Dictionary<string, (long MaxSize, string[] Extensions)> AllowedTypes = new()
    {
        ["image"] = (10 * 1024 * 1024, new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" }),
        ["video"] = (50 * 1024 * 1024, new[] { ".mp4", ".webm", ".mov" }),
        ["audio"] = (20 * 1024 * 1024, new[] { ".mp3", ".wav", ".ogg", ".m4a" }),
        ["pdf"] = (20 * 1024 * 1024, new[] { ".pdf" })
    };

    public override void Configure()
    {
        Post("/module/media");
        AllowFileUploads();
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UploadMediaRequest req, CancellationToken ct)
    {
        var file = req.File;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        string? matchedType = null;
        foreach (var (type, config) in AllowedTypes)
        {
            if (config.Extensions.Contains(ext))
            {
                matchedType = type;
                if (file.Length > config.MaxSize)
                {
                    ThrowError($"File too large. Max size for {type}: {config.MaxSize / (1024 * 1024)}MB");
                }
                break;
            }
        }

        if (matchedType is null)
        {
            ThrowError($"File type '{ext}' is not allowed.");
        }

        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(savePath, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        Response = new UploadMediaResponse { Url = $"/uploads/{fileName}" };
    }
}
