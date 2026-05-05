namespace Application.Interfaces.Services.Equipe.Dto;

public class EquipeDto
{
    public string Id { get; set; } = null!;

    public string NameFr { get; set; } = null!;
    public string NameEn { get; set; } = null!;

    public List<string> MemberUserIds { get; set; } = new();

    public string? ParentEquipeId { get; set; }
    public List<EquipeDto> SousEquipes { get; set; } = new();
}
