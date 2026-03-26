using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Application.Interfaces.Services.Equipe;
using Application.Interfaces.Services.Equipe.Dto;

public class GetAllEquipesEndpoint: EndpointWithoutRequest<List<EquipeDto>>
{
    private readonly IEquipeService _equipeService;

    public GetAllEquipesEndpoint(IEquipeService equipeService)
    {
        _equipeService = equipeService;
    }

    public override void Configure()
    {
        Get("/equipes");
        Roles(Domain.Constants.User.Roles.ADMINISTRATOR);
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        DontCatchExceptions();
    }

    public override async Task<List<EquipeDto>> ExecuteAsync(CancellationToken ct)
    {
        return await _equipeService.GetAllEquipes();
    }
}