namespace Web.Dtos;

public class MemberModuleDto
{
    public string ModuleId { get; set; } = null!;
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public string? NameFr { get; set; }
    public string? NameEn { get; set; }
    public string? SujetFr { get; set; }
    public string? SujetEn { get; set; }
    public string? CardImageUrl { get; set; }
    public int ProgressPercent { get; set; }
    public bool IsCompleted { get; set; }
}
