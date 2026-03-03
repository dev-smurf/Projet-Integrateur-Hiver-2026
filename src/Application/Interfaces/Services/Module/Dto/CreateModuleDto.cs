using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services.Module.Dto;

public class CreateModuleDto
{
    public string NameFr { get; set; } = null!;
    public string SujetFr { get; set; } = null!;
    public string ContenueFr { get; set; } = null!;

    public string? NameEn { get; set; }
    public string? SujetEn { get; set; }
    public string? ContenueEn { get; set; }

    public IFormFile? CardImage { get; set; }
}
