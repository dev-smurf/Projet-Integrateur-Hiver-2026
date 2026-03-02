using FastEndpoints;
using Application.Interfaces.Services.Module;
using Application.Interfaces.Services.Module.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class GetAllModulesEndpoint : EndpointWithoutRequest<List<ModuleDto>>
{
    private readonly IModuleService _moduleService;

    public GetAllModulesEndpoint(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    public override void Configure()
    {
        Get("/modules");
        AllowAnonymous();
    }

    public override async Task<List<ModuleDto>> ExecuteAsync(CancellationToken ct)
    {
        return await _moduleService.getAllModules();
    }
}