
using Domain.Repositories;

namespace Infrastructure.Repositories.Module;

public class ModuleRepository: IModuleRepository
{
    
    public ModuleRepository()
    {
        
    }

    public List<System.Reflection.Module> GetModules()
    {

        // j'ai pas pull entity
     return new List<System.Reflection.Module>();   
    }
}