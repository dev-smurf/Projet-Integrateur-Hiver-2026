namespace Application.Interfaces.Services.Equipe.Dto;

public class EquipeDto
{
    public string Id { get; set; } = null!;

    public string NameFr { get; set; } = null!;
    public string NameEn { get; set; } = null!;

    public string? ParentEquipeId { get; set; }
    public string? ParentEquipeNameFr { get; set; }
    public string? ParentEquipeNameEn { get; set; }

    public List<string> MemberIds { get; set; } = new();
    public List<string> MemberUserIds { get; set; } = new();
    public List<EquipeDto> SousEquipes { get; set; } = new();
}
