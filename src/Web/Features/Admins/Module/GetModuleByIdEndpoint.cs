using Domain.Repositories;
using FastEndpoints;
using Application.Interfaces.Services.Module.Dto;

namespace Web.Features.Admins.Module;

public class GetModuleByIdEndpoint : EndpointWithoutRequest<ModuleDto>
{
    private readonly IModuleRepository _repository;

    public GetModuleByIdEndpoint(IModuleRepository repository)
    {
        _repository = repository;
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

        var entity = await _repository.GetByIdAsync(guidId);

        if (entity is null)
        {
            HttpContext.Response.StatusCode = 404;
            Response = null!;
            return;
        }

        Response = new ModuleDto
        {
            Id = entity.Id.ToString(),
            NameFr = entity.NameFr,
            NameEn = entity.NameEn,
            SujetFr = entity.SujetFr,
            SujetEn = entity.SujetEn,
            ContenueFr = entity.ContenueFr,
            ContenueEn = entity.ContenueEn,
            CardImageUrl = entity.CardImageUrl
        };
    }
}
