using Domain.Entities;

namespace Domain.Repositories;

public interface IModuleRepository
{   
    Task Create(Module module);
 
}