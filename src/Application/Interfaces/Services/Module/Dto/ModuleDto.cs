namespace Application.Interfaces.Services.Module.Dto;

public class ModuleDto
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public string? CardImageUrl { get; set; }
    public List<ModuleSectionDto> Sections { get; set; } = new();
}
