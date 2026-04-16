using Domain.Extensions;
using Web.Features.Common;

public class CreateEquipeRequest : ISanitizable
{
    public string? NameFr { get; set; }
    public string? NameEn { get; set; }
    public List<string> MemberUserIds { get; set; } = new();

    public void Sanitize()
    {
        NameFr = NameFr?.Trim() ?? string.Empty;
        NameEn = NameEn?.Trim() ?? string.Empty;
    }
}

