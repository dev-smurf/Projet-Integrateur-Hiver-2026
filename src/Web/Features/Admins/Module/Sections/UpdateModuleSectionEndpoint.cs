using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Features.Admins.Module.Sections;

public class UpdateSectionRequest
{
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public int SortOrder { get; set; }
}

public class UpdateModuleSectionEndpoint : Endpoint<UpdateSectionRequest, SucceededOrNotResponse>
{
    private readonly IModuleSectionRepository _repository;

    public UpdateModuleSectionEndpoint(IModuleSectionRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/module/{moduleId}/sections/{sectionId}");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    }

    public override async Task HandleAsync(UpdateSectionRequest req, CancellationToken ct)
    {
        var sectionIdString = Route<string>("sectionId");
        if (!Guid.TryParse(sectionIdString, out var sectionId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = new SucceededOrNotResponse(false,
                new Error { ErrorType = "InvalidId", ErrorMessage = "ID invalide." });
            return;
        }

        var section = await _repository.GetByIdAsync(sectionId);
        if (section is null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(false,
                new Error { ErrorType = "NotFound", ErrorMessage = "Section introuvable." });
            return;
        }

        section.Title = req.Title;
        section.Content = req.Content;
        section.SortOrder = req.SortOrder;

        await _repository.UpdateAsync(section);
        Response = new SucceededOrNotResponse(true);
    }
}
