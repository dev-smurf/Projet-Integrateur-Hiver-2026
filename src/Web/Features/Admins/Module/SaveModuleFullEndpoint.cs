using Domain.Common;
using Domain.Entities;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Web.Features.Admins.Module;

public class SectionPayload
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public int SortOrder { get; set; }
    public bool IsDeleted { get; set; }
}

public class SaveModuleFullRequest
{
    public string? Name { get; set; }
    public string? Subject { get; set; }
    public string? Content { get; set; }
    public List<SectionPayload> Sections { get; set; } = new();
}

public class SaveModuleFullEndpoint : Endpoint<SaveModuleFullRequest, SucceededOrNotResponse>
{
    private readonly GarneauTemplateDbContext _context;

    public SaveModuleFullEndpoint(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Put("/module/{id}/full");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(SaveModuleFullRequest req, CancellationToken ct)
    {
        var idString = Route<string>("id");

        if (!Guid.TryParse(idString, out var moduleId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = new SucceededOrNotResponse(false,
                new Error { ErrorType = "InvalidId", ErrorMessage = "ID invalide." });
            return;
        }

        var module = await _context.Modules
            .Include(m => m.Sections)
            .FirstOrDefaultAsync(m => m.Id == moduleId, ct);

        if (module is null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(false,
                new Error { ErrorType = "NotFound", ErrorMessage = "Module introuvable." });
            return;
        }

        // Update module metadata
        if (!string.IsNullOrWhiteSpace(req.Name))
            module.Name = req.Name;
        if (req.Subject is not null)
            module.Subject = req.Subject;
        if (req.Content is not null)
            module.Content = req.Content;

        // Diff sections
        var existingSections = module.Sections.Where(s => s.Deleted == null).ToDictionary(s => s.Id);

        foreach (var sectionPayload in req.Sections)
        {
            if (sectionPayload.Id.HasValue && existingSections.TryGetValue(sectionPayload.Id.Value, out var existing))
            {
                if (sectionPayload.IsDeleted)
                {
                    existing.SoftDelete();
                }
                else
                {
                    existing.Title = sectionPayload.Title;
                    existing.Content = sectionPayload.Content;
                    existing.SortOrder = sectionPayload.SortOrder;
                }
            }
            else if (!sectionPayload.IsDeleted)
            {
                var newSection = new ModuleSection
                {
                    ModuleId = moduleId,
                    Title = sectionPayload.Title,
                    Content = sectionPayload.Content,
                    SortOrder = sectionPayload.SortOrder
                };
                _context.ModuleSections.Add(newSection);
            }
        }

        await _context.SaveChangesAsync(ct);
        Response = new SucceededOrNotResponse(true);
    }
}
