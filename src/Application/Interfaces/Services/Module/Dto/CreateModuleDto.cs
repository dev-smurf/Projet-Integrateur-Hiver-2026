using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services.Module.Dto;

public class CreateModuleDto
{
    public string Name { get; set; } = null!;
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public IFormFile? CardImage { get; set; }
}
