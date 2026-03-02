using Domain.Repositories;
using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Module;

public class ModuleRepository : IModuleRepository
{
    private readonly GarneauTemplateDbContext _context;

    public ModuleRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Domain.Entities.Module>> GetAllAsync()
    {
        return await _context.Modules
            .Where(m => m.Deleted == null)
            .ToListAsync();
    }

    public async Task<Domain.Entities.Module?> GetByIdAsync(Guid id)
    {
        return await _context.Modules.FindAsync(id);
    }

    public async Task Create(Domain.Entities.Module module)
    {
        _context.Modules.Add(module);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Entities.Module module)
    {
        _context.Modules.Update(module);
        await _context.SaveChangesAsync();
    }

    public async Task<Domain.Entities.Module> AddAsync(Domain.Entities.Module module)
    {
        _context.Modules.Add(module);
        await _context.SaveChangesAsync();
        return module;
    }
}