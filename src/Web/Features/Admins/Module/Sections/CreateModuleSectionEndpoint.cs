using Application.Interfaces.Services.Module.Dto;
using Domain.Entities;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Module.Sections;

public class CreateSectionRequest
{
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public int SortOrder { get; set; }
}

public class CreateModuleSectionEndpoint : Endpoint<CreateSectionRequest, ModuleSectionDto>
{
    private readonly IModuleSectionRepository _repository;

    public CreateModuleSectionEndpoint(IModuleSectionRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/module/{moduleId}/sections");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CreateSectionRequest req, CancellationToken ct)
    {
        var moduleIdString = Route<string>("moduleId");
        if (!Guid.TryParse(moduleIdString, out var moduleId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var section = new ModuleSection
        {
            ModuleId = moduleId,
            Title = req.Title,
            Content = req.Content,
            SortOrder = req.SortOrder
        };

        await _repository.AddAsync(section);

        Response = new ModuleSectionDto
        {
            Id = section.Id.ToString(),
            Title = section.Title,
            Content = section.Content,
            SortOrder = section.SortOrder
        };
    }
}
