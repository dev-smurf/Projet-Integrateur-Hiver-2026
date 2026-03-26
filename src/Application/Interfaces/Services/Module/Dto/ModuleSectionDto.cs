namespace Application.Interfaces.Services.Module.Dto;

public class ModuleSectionDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public int SortOrder { get; set; }
}
