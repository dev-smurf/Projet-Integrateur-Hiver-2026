using Microsoft.AspNetCore.Http;

namespace Web.Features.Admins.Equipe;

public class EditEquipeRequest : IEditEquipeRequest
{
    public string Id { get; set; } = null!;
    public string NameFr { get; set; } = null!;
    public string NameEn { get; set; } = null!;
    public List<string> MemberIds { get; set; } = new();
    public string? ParentEquipeId { get; set; }
}
