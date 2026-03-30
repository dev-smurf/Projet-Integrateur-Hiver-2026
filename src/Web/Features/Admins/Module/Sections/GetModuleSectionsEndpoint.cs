using Application.Interfaces.Services.Module.Dto;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Module.Sections;

public class GetModuleSectionsEndpoint : EndpointWithoutRequest<List<ModuleSectionDto>>
{
    private readonly IModuleSectionRepository _repository;

    public GetModuleSectionsEndpoint(IModuleSectionRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/module/{moduleId}/sections");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var moduleIdString = Route<string>("moduleId");
        if (!Guid.TryParse(moduleIdString, out var moduleId))
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        var sections = await _repository.GetByModuleIdAsync(moduleId);
        Response = sections.Select(s => new ModuleSectionDto
        {
            Id = s.Id.ToString(),
            Title = s.Title,
            Content = s.Content,
            SortOrder = s.SortOrder
        }).ToList();
    }
}
