using Domain.Entities;

namespace Domain.Repositories;

public interface IModuleSectionRepository
{
    Task<IEnumerable<ModuleSection>> GetByModuleIdAsync(Guid moduleId);
    Task<ModuleSection?> GetByIdAsync(Guid id);
    Task<ModuleSection> AddAsync(ModuleSection section);
    Task UpdateAsync(ModuleSection section);
    Task DeleteAsync(ModuleSection section);
}
