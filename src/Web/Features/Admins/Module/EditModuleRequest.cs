using Microsoft.AspNetCore.Http;

namespace Web.Features.Admins.Module;

public class EditModuleRequest : IEditModuleRequest
{
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public IFormFile? CardImage { get; set; }
}
