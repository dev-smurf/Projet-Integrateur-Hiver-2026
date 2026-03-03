using Microsoft.AspNetCore.Http;

namespace Web.Features.Admins.Module;

public class EditModuleRequest : IEditModuleRequest
{
    public string? NameFr { get; set; }
    public string? NameEn { get; set; }
    public string? SujetFr { get; set; }
    public string? SujetEn { get; set; }
    public string? ContenueFr { get; set; }
    public string? ContenueEn { get; set; }
    public IFormFile? CardImage { get; set; }
}
