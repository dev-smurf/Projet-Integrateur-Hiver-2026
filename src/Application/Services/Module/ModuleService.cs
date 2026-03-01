
using Application.Interfaces.Services.Module;
using Application.Services.Module.Dto;
using AutoMapper;
using Domain.Repositories;

namespace Application.Services.Module;
public class ModuleService:IModuleService
{
    private readonly IMapper _mapper;   
    private readonly IModuleService _moduleService;
    private readonly IModuleRepository _moduleRepository;

    public ModuleService(IMapper mapper, ModuleService moduleService, IModuleRepository moduleRepository)
    {
        _mapper = mapper;
        _moduleService = moduleService;
        _moduleRepository = moduleRepository;
    }
public async Task<Domain.Entities.Module> CreateModule(Domain.Entities.Module module)
{
    var newModule = await _moduleService.CreateModule(module);
    return newModule;
}

}