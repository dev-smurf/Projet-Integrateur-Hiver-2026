using Application.Interfaces.Services.Module;
using Application.Interfaces.Services.Module.Dto;
using Domain.Repositories;

namespace Application.Services.Module
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository;

        public ModuleService(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        public async Task<List<ModuleDto>> getAllModules()
        {
            var modules = await _moduleRepository.GetAllAsync();

            return modules.Select(m => new ModuleDto
            {
                Id = m.Id.ToString(),
                NameFr = m.NameFr,
                NameEn = m.NameEn,
                SujetFr = m.SujetFr,
                SujetEn = m.SujetEn,
                ContenueFr = m.ContenueFr,
                ContenueEn = m.ContenueEn,
                CardImageUrl = m.CardImageUrl
            }).ToList();
        }

        public async Task<ModuleDto?> getModule(string id)
        {
            if (!Guid.TryParse(id, out Guid guidId))
                return null;

            var module = await _moduleRepository.GetByIdAsync(guidId);
            if (module == null)
                return null;

            return new ModuleDto
            {
                Id = module.Id.ToString(),
                NameFr = module.NameFr,
                NameEn = module.NameEn,
                SujetFr = module.SujetFr,
                SujetEn = module.SujetEn,
                ContenueFr = module.ContenueFr,
                ContenueEn = module.ContenueEn,
                CardImageUrl = module.CardImageUrl
            };
        }

        public async Task<ModuleDto> CreateModule(CreateModuleDto request)
        {
            var module = new Domain.Entities.Module
            {
                NameFr = request.NameFr,
                NameEn = request.NameEn,
                SujetFr = request.SujetFr,
                SujetEn = request.SujetEn,
                ContenueFr = request.ContenueFr,
                ContenueEn = request.ContenueEn,
                CardImageUrl = null
            };

            await _moduleRepository.AddAsync(module);

            return new ModuleDto
            {
                Id = module.Id.ToString(),
                NameFr = module.NameFr,
                NameEn = module.NameEn,
                SujetFr = module.SujetFr,
                SujetEn = module.SujetEn,
                ContenueFr = module.ContenueFr,
                ContenueEn = module.ContenueEn,
                CardImageUrl = module.CardImageUrl
            };
        }

        public async Task<bool> UpdateModule(string id, ModuleDto request)
        {
            if (!Guid.TryParse(id, out Guid guidId))
                return false;

            var existingModule = await _moduleRepository.GetByIdAsync(guidId);
            if (existingModule == null)
                return false;

            existingModule.NameFr = request.NameFr ?? existingModule.NameFr;
            existingModule.NameEn = request.NameEn ?? existingModule.NameEn;
            existingModule.SujetFr = request.SujetFr ?? existingModule.SujetFr;
            existingModule.SujetEn = request.SujetEn ?? existingModule.SujetEn;
            existingModule.ContenueFr = request.ContenueFr ?? existingModule.ContenueFr;
            existingModule.ContenueEn = request.ContenueEn ?? existingModule.ContenueEn;

            if (!string.IsNullOrWhiteSpace(request.CardImageUrl))
                existingModule.CardImageUrl = request.CardImageUrl;

            await _moduleRepository.UpdateAsync(existingModule);
            return true;
        }
    }
}
