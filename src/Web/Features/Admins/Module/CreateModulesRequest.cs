using Microsoft.AspNetCore.Http;
using Web.Features.Common;

public class CreateModulesRequest : ISanitizable
{
    public string? NameFr { get; set; }
    public string? NameEn { get; set; }
    public string? ContenueFr { get; set; }
    public string? ContenueEn { get; set; }
    public string? SujetFr { get; set; }
    public string? SujetEn { get; set; }
    
    public string? CardImageBase64 { get; set; } 
    public void Sanitize()
    {
        NameFr = NameFr?.Trim();
        NameEn = NameEn?.Trim();
        ContenueFr = ContenueFr?.Trim();
        ContenueEn = ContenueEn?.Trim();
        SujetFr = SujetFr?.Trim();
        SujetEn = SujetEn?.Trim();
    }
}

