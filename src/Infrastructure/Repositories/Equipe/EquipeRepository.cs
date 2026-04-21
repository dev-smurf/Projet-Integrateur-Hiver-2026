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

    public async Task<List<Guid>> GetEquipeIdsForUser(Guid userId)
    {
        return await _context.Equipes
            .Where(e => e.Deleted == null && e.Membres.Any(m => m.Id == userId))
            .Select(e => e.Id)
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

    public async Task ReplaceUserEquipes(User user, IEnumerable<Guid> ids)
    {
        var distinctIds = ids
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var currentEquipes = await _context.Equipes
            .Include(e => e.Membres)
            .Where(e => e.Deleted == null && e.Membres.Any(m => m.Id == user.Id))
            .ToListAsync();

        foreach (var equipe in currentEquipes)
        {
            var membre = equipe.Membres.FirstOrDefault(m => m.Id == user.Id);
            if (membre != null)
            {
                equipe.Membres.Remove(membre);
            }
        }

        if (distinctIds.Count > 0)
        {
            var equipesToAssign = await GetByIds(distinctIds);
            foreach (var equipe in equipesToAssign)
            {
                if (equipe.Membres.All(m => m.Id != user.Id))
                {
                    equipe.Membres.Add(user);
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Equipe?> FindById(Guid id)
    {
        return await _context.Equipes
            .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null);
    }

    public async Task<Equipe?> FindByIdWithMembers(Guid id)
    {
        return await _context.Equipes
            .Include(e => e.Membres)
            .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null);
    }

    public async Task SetEquipeMembers(Guid equipeId, IEnumerable<Guid> userIds)
    {
        var equipe = await _context.Equipes
            .Include(e => e.Membres)
            .FirstOrDefaultAsync(e => e.Id == equipeId && e.Deleted == null);

        if (equipe == null)
            return;

        var targetIds = userIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToHashSet();

        var toRemove = equipe.Membres.Where(user => !targetIds.Contains(user.Id)).ToList();
        foreach (var user in toRemove)
        {
            equipe.Membres.Remove(user);
        }

        var currentIds = equipe.Membres.Select(user => user.Id).ToHashSet();
        var toAddIds = targetIds.Where(id => !currentIds.Contains(id)).ToList();
        if (toAddIds.Count > 0)
        {
            var users = await _context.Users.Where(user => toAddIds.Contains(user.Id)).ToListAsync();
            foreach (var user in users)
            {
                equipe.Membres.Add(user);
            }
        }

        await _context.SaveChangesAsync();
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
