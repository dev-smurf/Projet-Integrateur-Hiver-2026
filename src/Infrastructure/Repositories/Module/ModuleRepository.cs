using Domain.Repositories;
using Persistence;

namespace Infrastructure.Repositories.Module;

public class ModuleRepository: IModuleRepository
{
    
    private readonly GarneauTemplateDbContext _context;

    public ModuleRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task Create(Domain.Entities.Module module)
    {
        _context.Modules.Add(module);
        await _context.SaveChangesAsync();
    }
}