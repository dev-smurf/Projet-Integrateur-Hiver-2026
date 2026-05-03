using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Repositories;
using Microsoft.Data.SqlClient;
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
            .Include(e => e.ParentEquipe)
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
        try
        {
            return await _context.Equipes
                .Include(e => e.Membres)
                .Include(e => e.MemberEquipes)
                    .ThenInclude(me => me.Member)
                        .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null);
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            return await _context.Equipes
                .Include(e => e.Membres)
                .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null);
        }
    }

    public async Task<Equipe?> FindByIdWithMembersAndSousEquipes(Guid id)
    {
        try
        {
            return await _context.Equipes
                .Include(e => e.Membres)
                .Include(e => e.MemberEquipes)
                    .ThenInclude(me => me.Member)
                        .ThenInclude(m => m.User)
                .Include(e => e.SousEquipes)
                .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null);
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            return await _context.Equipes
                .Include(e => e.Membres)
                .Include(e => e.SousEquipes)
                .FirstOrDefaultAsync(e => e.Id == id && e.Deleted == null);
        }
    }

    public async Task SetEquipeMembers(Guid equipeId, IEnumerable<Guid> userIds)
    {
        try
        {
            var equipe = await _context.Equipes
                .Include(e => e.Membres)
                .Include(e => e.MemberEquipes)
                .FirstOrDefaultAsync(e => e.Id == equipeId && e.Deleted == null);

            if (equipe == null)
                return;

            var targetMemberIds = userIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToHashSet();

            var currentMemberAssignments = await _context.MemberEquipes
                .Include(me => me.Member)
                    .ThenInclude(m => m.User)
                .Where(me => me.EquipeId == equipeId && me.Deleted == null)
                .ToListAsync();

            var toRemoveAssignments = currentMemberAssignments
                .Where(me => !targetMemberIds.Contains(me.MemberId))
                .ToList();

            foreach (var assignment in toRemoveAssignments)
            {
                assignment.SoftDelete();
                _context.MemberEquipes.Update(assignment);
            }

            var existingMemberIds = currentMemberAssignments
                .Select(me => me.MemberId)
                .ToHashSet();

            var toAddMemberIds = targetMemberIds
                .Where(id => !existingMemberIds.Contains(id))
                .ToList();

            if (toAddMemberIds.Count > 0)
            {
                foreach (var memberId in toAddMemberIds)
                {
                    _context.MemberEquipes.Add(new MemberEquipe(memberId, equipeId));
                }
            }

            var activeAssignments = currentMemberAssignments
                .Where(me => !toRemoveAssignments.Any(removed => removed.Id == me.Id))
                .ToList();

            var allAssignedMemberIds = activeAssignments
                .Select(me => me.MemberId)
                .Concat(toAddMemberIds)
                .Distinct()
                .ToList();

            var assignedUsers = await _context.Members
                .Include(m => m.User)
                .Where(m => allAssignedMemberIds.Contains(m.Id) && m.Deleted == null)
                .Select(m => m.User)
                .ToListAsync();

            equipe.Membres.Clear();
            foreach (var user in assignedUsers)
            {
                equipe.Membres.Add(user);
            }

            await _context.SaveChangesAsync();
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            var equipe = await _context.Equipes
                .Include(e => e.Membres)
                .FirstOrDefaultAsync(e => e.Id == equipeId && e.Deleted == null);

            if (equipe == null)
                return;

            var targetMemberIds = userIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var assignedUsers = await _context.Members
                .Include(m => m.User)
                .Where(m => targetMemberIds.Contains(m.Id) && m.Deleted == null)
                .Select(m => m.User)
                .ToListAsync();

            equipe.Membres.Clear();
            foreach (var user in assignedUsers)
            {
                equipe.Membres.Add(user);
            }

            await _context.SaveChangesAsync();
        }
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
