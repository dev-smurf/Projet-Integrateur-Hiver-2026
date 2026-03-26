using Microsoft.AspNetCore.Http;
using Web.Features.Common;

public class CreateModulesRequest : ISanitizable
{
    public string? Name { get; set; }
    public string? Content { get; set; }
    public string? Subject { get; set; }

    public IFormFile? CardImage { get; set; }

    public void Sanitize()
    {
        Name = Name?.Trim();
        Content = Content?.Trim();
        Subject = Subject?.Trim();
    }
}
