using Domain.Entities;
using Domain.Entities.Identity;
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

    public async Task<List<Equipe>> GetByIds(IEnumerable<Guid> ids)
    {
        var distinctIds = ids
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (distinctIds.Count == 0)
        {
            return [];
        }

        return await _context.Equipes
            .Include(e => e.Membres)
            .Where(e => e.Deleted == null && distinctIds.Contains(e.Id))
            .ToListAsync();
    }

    public async Task AssignUserToEquipes(User user, IEnumerable<Guid> ids)
    {
        var equipes = await GetByIds(ids);

        if (equipes.Count == 0)
        {
            return;
        }

        foreach (var equipe in equipes)
        {
            if (equipe.Membres.All(membre => membre.Id != user.Id))
            {
                equipe.Membres.Add(user);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Equipe?> FindById(Guid id)
    {
        return await _context.Equipes
            .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null);
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
