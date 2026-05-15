using System;
using System.Collections.Generic;

namespace Domain.Repositories
{
    public interface IEquipeModuleRepository
    {
        Task<EquipeModule> AssignAsync(EquipeModule equipeModule);

        Task UnassignAsync(EquipeModule equipeModule);

        Task<bool> IsAssignedAsync(Guid equipeId, Guid moduleId);

        Task<EquipeModule?> GetByEquipeAndModuleAsync(Guid equipeId, Guid moduleId);

        Task<List<EquipeModule>> GetByEquipeIdAsync(Guid equipeId);

        Task<List<EquipeModule>> GetByModuleIdAsync(Guid moduleId);
    }
}
