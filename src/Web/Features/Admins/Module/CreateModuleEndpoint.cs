using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Domain.Entities;
using Domain.Repositories;
using Domain.Common;

public class CreateModuleEndpoint : Endpoint<CreateModulesRequest, SucceededOrNotResponse>
{
    private readonly IModuleRepository _moduleService;

    public CreateModuleEndpoint(IModuleRepository moduleService)
    {
        _moduleService = moduleService;
    }

    public override void Configure()
    {
        AllowFileUploads();
        Post("module");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CreateModulesRequest req, CancellationToken ct)
    {
        req.Sanitize();

        string? cardImageUrl = null;
        if (!string.IsNullOrEmpty(req.CardImageBase64))
        {
            cardImageUrl = await SaveFileFromBase64(req.CardImageBase64);
        }

        var newModule = new Module
        {
            NameFr = req.NameFr,
            NameEn = string.IsNullOrWhiteSpace(req.NameEn) ? req.NameFr : req.NameEn,           // 👈
            ContenueFr = req.ContenueFr,
            ContenueEn = string.IsNullOrWhiteSpace(req.ContenueEn) ? req.ContenueFr : req.ContenueEn, // 👈
            SujetFr = req.SujetFr,
            SujetEn = string.IsNullOrWhiteSpace(req.SujetEn) ? req.SujetFr : req.SujetEn,       // 👈
            CardImageUrl = cardImageUrl
        };

        await _moduleService.Create(newModule);
        await Send.OkAsync(new SucceededOrNotResponse(true));
    }

    private static async Task<string> SaveFileFromBase64(string base64, string folder = "uploads")
    {
        var bytes = Convert.FromBase64String(base64);
        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        var fileName = $"{Guid.NewGuid()}.png";
        var fullPath = Path.Combine(savePath, fileName);
        await File.WriteAllBytesAsync(fullPath, bytes);

        return $"/{folder}/{fileName}";
    }
}