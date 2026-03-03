namespace Application.Interfaces.Services.Module.Dto;

public class ModuleDto
{
    public string Id { get; set; } = null!;

    public string? NameFr { get; set; }
    public string? NameEn { get; set; }

    public string? SujetFr { get; set; }
    public string? SujetEn { get; set; }

    public string? ContenueFr { get; set; }
    public string? ContenueEn { get; set; }

    public string? CardImageUrl { get; set; }
}
