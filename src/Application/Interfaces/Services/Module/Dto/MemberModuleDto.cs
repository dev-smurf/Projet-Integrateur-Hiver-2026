namespace Application.Interfaces.Services.Module.Dto;

public class MemberModuleDto
{
    public string Id { get; set; } = null!;
    public string MemberId { get; set; } = null!;
    public string ModuleId { get; set; } = null!;
    public string? MemberName { get; set; }
}
