using Domain.Repositories;
using FastEndpoints;
using Application.Interfaces.Services.Module.Dto;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Web.Features.Admins.Module;

public class GetModuleByIdEndpoint : EndpointWithoutRequest<ModuleDto>
{
    private readonly GarneauTemplateDbContext _context;

    public GetModuleByIdEndpoint(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/module/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var idString = Route<string>("id");

        if (!Guid.TryParse(idString, out var guidId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = null!;
            return;
        }

        var entity = await _context.Modules
            .Include(m => m.Sections.Where(s => s.Deleted == null).OrderBy(s => s.SortOrder))
            .FirstOrDefaultAsync(m => m.Id == guidId, ct);

        if (entity is null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = null!;
            return;
        }

        Response = new ModuleDto
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Subject = entity.Subject,
            Content = entity.Content,
            CardImageUrl = entity.CardImageUrl,
            Sections = entity.Sections.Select(s => new ModuleSectionDto
            {
                Id = s.Id.ToString(),
                Title = s.Title,
                Content = s.Content,
                SortOrder = s.SortOrder
            }).ToList()
        };
    }
}
