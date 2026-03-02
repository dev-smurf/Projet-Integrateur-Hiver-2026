using Application.Interfaces.Services.Module;
using Application.Interfaces.Services.Module.Dto;
using Domain.Entities;
using Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Module
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;

        public ModuleService(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        public async Task<ModuleDto> CreateModule(CreateModuleDto request)
        {
            var module = new Domain.Entities.Module
            {
                NameFr = request.NameFr,
                ContenueFr = request.ContenueFr,
                SujetFr = request.SujetFr
            };

            var newModule = await _moduleRepository.AddAsync(module);

            return new ModuleDto
            {
                Id = newModule.Id.ToString(),
                nameFr = newModule.NameFr ?? "",
                sujetFr = newModule.SujetFr ?? ""
            };
        }

        public async Task<List<ModuleDto>> GetAllModules()
        {
            var modules = await _moduleRepository.GetAllAsync();

            return modules.Select(m => new ModuleDto
            {
                Id = m.Id.ToString(),
                nameFr = m.NameFr ?? "",
                sujetFr = m.SujetFr ?? ""
            }).ToList();
        }
    }
}