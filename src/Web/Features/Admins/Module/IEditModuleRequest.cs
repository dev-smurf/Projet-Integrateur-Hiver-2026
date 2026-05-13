using Microsoft.AspNetCore.Http;

public interface IEditModuleRequest
{
    string? Name { get; set; }
    string? Subject { get; set; }
    string? Content { get; set; }
    bool? IsPublished { get; set; }
    IFormFile? CardImage { get; set; }
}
