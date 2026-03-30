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
                Name = m.Name,
                Subject = m.Subject,
                Content = m.Content,
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
                Name = module.Name,
                Subject = module.Subject,
                Content = module.Content,
                CardImageUrl = module.CardImageUrl
            };
        }

        public async Task<ModuleDto> CreateModule(CreateModuleDto request)
        {
            var module = new Domain.Entities.Module
            {
                Name = request.Name,
                Subject = request.Subject,
                Content = request.Content,
                CardImageUrl = null
            };

            await _moduleRepository.AddAsync(module);

            return new ModuleDto
            {
                Id = module.Id.ToString(),
                Name = module.Name,
                Subject = module.Subject,
                Content = module.Content,
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

            existingModule.Name = request.Name ?? existingModule.Name;
            existingModule.Subject = request.Subject ?? existingModule.Subject;
            existingModule.Content = request.Content ?? existingModule.Content;

            if (!string.IsNullOrWhiteSpace(request.CardImageUrl))
                existingModule.CardImageUrl = request.CardImageUrl;

            await _moduleRepository.UpdateAsync(existingModule);
            return true;
        }
    }
}
