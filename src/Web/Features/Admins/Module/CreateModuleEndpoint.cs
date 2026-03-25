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
        Post("modules");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task HandleAsync(CreateModulesRequest req, CancellationToken ct)
    {
        req.Sanitize();

        string? cardImageUrl = null;
        if (req.CardImage is not null)
        {
            cardImageUrl = await SaveFile(req.CardImage);
        }

        var newModule = new Module
        {
            Name = req.Name,
            Content = req.Content,
            Subject = req.Subject,
            CardImageUrl = cardImageUrl
        };

        await _moduleService.Create(newModule);
        await Send.OkAsync(new SucceededOrNotResponse(true));
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
