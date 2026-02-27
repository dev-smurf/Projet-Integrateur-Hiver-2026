using System;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Repositories;

public interface IModuleRepository
{
    Task<Module?> GetByIdAsync(Guid id);

    Task Create(Module module);

    Task UpdateAsync(Domain.Entities.Module module);
}