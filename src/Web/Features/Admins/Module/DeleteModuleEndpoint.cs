using Domain.Common;
using Domain.Repositories;
using FastEndpoints;

namespace Web.Features.Admins.Module;

public class DeleteModuleEndpoint : EndpointWithoutRequest<SucceededOrNotResponse>
{
    private readonly IModuleRepository _repository;

    public DeleteModuleEndpoint(IModuleRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Delete("/module/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var idString = Route<string>("id");

        if (!Guid.TryParse(idString, out var guidId))
        {
            HttpContext.Response.StatusCode = 400;
            Response = new SucceededOrNotResponse(
                false,
                new Error { ErrorType = "InvalidId", ErrorMessage = "ID invalide." }
            );
            return;
        }

        var entity = await _repository.GetByIdAsync(guidId);

        if (entity is null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = new SucceededOrNotResponse(
                false,
                new Error { ErrorType = "NotFound", ErrorMessage = "Module introuvable." }
            );
            return;
        }

        entity.SoftDelete();
        await _repository.DeleteAsync(entity);

        Response = new SucceededOrNotResponse(true);
    }
}
