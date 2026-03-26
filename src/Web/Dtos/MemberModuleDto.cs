namespace Web.Dtos;

public class MemberModuleDto
{
    public string ModuleId { get; set; } = null!;
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public string? CardImageUrl { get; set; }
    public int ProgressPercent { get; set; }
    public bool IsCompleted { get; set; }
}
