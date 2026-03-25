using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Module.Sections;

public class ReorderSectionsRequest
{
    public List<Guid> SectionIds { get; set; } = new();
}

public class ReorderModuleSectionsEndpoint : Endpoint<ReorderSectionsRequest, SucceededOrNotResponse>
{
    private readonly IModuleSectionRepository _repository;

    public ReorderModuleSectionsEndpoint(IModuleSectionRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/module/{moduleId}/sections/reorder");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(ReorderSectionsRequest req, CancellationToken ct)
    {
        var moduleIdString = Route<string>("moduleId");
        if (!Guid.TryParse(moduleIdString, out var moduleId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = new SucceededOrNotResponse(false,
                new Error { ErrorType = "InvalidId", ErrorMessage = "ID invalide." });
            return;
        }

        var sections = await _repository.GetByModuleIdAsync(moduleId);
        var sectionDict = sections.ToDictionary(s => s.Id);

        for (int i = 0; i < req.SectionIds.Count; i++)
        {
            if (sectionDict.TryGetValue(req.SectionIds[i], out var section))
            {
                section.SortOrder = i;
            }
        }

        foreach (var section in sectionDict.Values)
        {
            await _repository.UpdateAsync(section);
        }

        Response = new SucceededOrNotResponse(true);
    }
}
