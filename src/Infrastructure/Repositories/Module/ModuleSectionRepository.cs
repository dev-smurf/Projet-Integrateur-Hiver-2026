using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Module;

public class ModuleSectionRepository : IModuleSectionRepository
{
    private readonly GarneauTemplateDbContext _context;

    public ModuleSectionRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ModuleSection>> GetByModuleIdAsync(Guid moduleId)
    {
        return await _context.ModuleSections
            .Where(s => s.ModuleId == moduleId && s.Deleted == null)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
    }

    public async Task<ModuleSection?> GetByIdAsync(Guid id)
    {
        return await _context.ModuleSections
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ModuleSection> AddAsync(ModuleSection section)
    {
        _context.ModuleSections.Add(section);
        await _context.SaveChangesAsync();
        return section;
    }

    public async Task UpdateAsync(ModuleSection section)
    {
        _context.ModuleSections.Update(section);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ModuleSection section)
    {
        _context.ModuleSections.Remove(section);
        await _context.SaveChangesAsync();
    }
}
