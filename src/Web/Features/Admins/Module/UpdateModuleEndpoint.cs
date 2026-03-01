using Domain.Common;
using Domain.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Web.Features.Admins.Module;

public class UpdateModuleEndpoint : Endpoint<IEditModuleRequest, SucceededOrNotResponse>
{
    private readonly IModuleRepository _repository;

    public UpdateModuleEndpoint(IModuleRepository repository) => _repository = repository;

    public override void Configure()
    {
        Put("/module/{id}");
        AllowFileUploads();
        AllowAnonymous();
    }

    public override async Task HandleAsync(IEditModuleRequest req, CancellationToken ct)
    {
        var idString = Route<string>("id");


        if (!Guid.TryParse(idString, out var guidId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = new SucceededOrNotResponse(false);
            return;
        }


        var entity = await _repository.GetByIdAsync(guidId);

        if (entity == null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(false);
            return;
        }


        entity.NameFr = req.NameFr;
        entity.ContenueFr = req.ContenueFr;
        entity.SujetFr = req.SujetFr;


        entity.SanitizeForSaving();
        await _repository.UpdateAsync(entity);

        Response = new SucceededOrNotResponse(true);
    }
}