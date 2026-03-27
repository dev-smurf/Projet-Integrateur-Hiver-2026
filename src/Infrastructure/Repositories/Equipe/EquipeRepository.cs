using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

public class EquipeRepository : IEquipeRepository
{
    private readonly GarneauTemplateDbContext _context;

    public EquipeRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<List<Equipe>> GetAll()
    {
        return await _context.Equipes
            .Where(e => e.Deleted == null)
            .ToListAsync();
    }

    public async Task<Equipe?> FindById(Guid id)
    {
        return await _context.Equipes
            .Include(e => e.Membres)
            .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null);
    }

    public async Task<Equipe?> FindByUserId(Guid userId)
    {
        return await _context.Equipes
            .Include(e => e.Membres)
            .Where(e => e.Deleted == null)
            .FirstOrDefaultAsync(e => e.Membres.Any(m => m.Id == userId));
    }

    public async Task CreateEquipe(Equipe equipe)
    {
        await _context.Equipes.AddAsync(equipe);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEquipe(Equipe equipe)
    {
        _context.Equipes.Update(equipe);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEquipeWithId(Guid id)
    {
        var equipe = await FindById(id);
        if (equipe != null)
        {
            equipe.SoftDelete();
            _context.Equipes.Update(equipe);
            await _context.SaveChangesAsync();
        }
    }
}