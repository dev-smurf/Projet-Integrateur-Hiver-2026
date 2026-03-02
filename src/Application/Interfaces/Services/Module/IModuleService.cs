using Application.Interfaces.Services.Module.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Module
{
    public interface IModuleService
    {
        Task<ModuleDto> CreateModule(CreateModuleDto request);
        Task<List<ModuleDto>> GetAllModules();
    }
}