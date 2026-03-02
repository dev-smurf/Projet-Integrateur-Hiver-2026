using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Repositories;

public interface IModuleRepository
{
    Task<IEnumerable<Module>> GetAllAsync();

    Task<Module?> GetByIdAsync(Guid id);

    Task Create(Module module);

    Task UpdateAsync(Module module);

    Task<Module> AddAsync(Module module);
}