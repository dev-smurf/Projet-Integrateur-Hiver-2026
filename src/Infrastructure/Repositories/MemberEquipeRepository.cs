using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Equipe;

// The database currently persists team membership via the many-to-many join table configured in
// EquipeConfiguration (Equipe <-> Identity.User). This repository adapts that storage model to the
// domain-level "Member <-> Equipe" operations used by the Web endpoints.
public class MemberEquipeRepository : IMemberEquipeRepository
{
    private readonly GarneauTemplateDbContext _context;

    public MemberEquipeRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task AssignAsync(MemberEquipe memberEquipe)
    {
        var member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == memberEquipe.MemberId);
        if (member == null)
        {
            return;
        }

        var equipe = await _context.Equipes
            .Include(e => e.Membres)
            .FirstOrDefaultAsync(e => e.Id == memberEquipe.EquipeId);
        if (equipe == null)
        {
            return;
        }

        var userId = member.User.Id;
        if (equipe.Membres.All(u => u.Id != userId))
        {
            equipe.Membres.Add(member.User);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnassignAsync(MemberEquipe memberEquipe)
    {
        var member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == memberEquipe.MemberId);
        if (member == null)
        {
            return;
        }

        var equipe = await _context.Equipes
            .Include(e => e.Membres)
            .FirstOrDefaultAsync(e => e.Id == memberEquipe.EquipeId);
        if (equipe == null)
        {
            return;
        }

        var userId = member.User.Id;
        var existingUser = equipe.Membres.FirstOrDefault(u => u.Id == userId);
        if (existingUser != null)
        {
            equipe.Membres.Remove(existingUser);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<MemberEquipe>> GetByEquipeIdAsync(Guid equipeId)
    {
        var equipe = await _context.Equipes
            .Include(e => e.Membres)
            .FirstOrDefaultAsync(e => e.Id == equipeId);

        if (equipe == null)
        {
            return [];
        }

        var userIds = equipe.Membres.Select(u => u.Id).Distinct().ToList();
        if (userIds.Count == 0)
        {
            return [];
        }

        var members = await _context.Members
            .Include(m => m.User)
            .Where(m => userIds.Contains(m.User.Id))
            .ToListAsync();

        // Note: no persisted join entity id exists with the current schema; we return ephemeral ids.
        return members.Select(m => new MemberEquipe(m.Id, equipeId) { Member = m }).ToList();
    }

    public async Task<IEnumerable<MemberEquipe>> GetByMemberIdAsync(Guid memberId)
    {
        var member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == memberId);
        if (member == null)
        {
            return [];
        }

        var userId = member.User.Id;

        var equipeIds = await _context.Equipes
            .Where(e => e.Membres.Any(u => u.Id == userId))
            .Select(e => e.Id)
            .ToListAsync();

        return equipeIds.Select(equipeId => new MemberEquipe(memberId, equipeId) { Member = member }).ToList();
    }

    public async Task<MemberEquipe?> GetByMemberAndEquipeAsync(Guid memberId, Guid equipeId)
    {
        var member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == memberId);
        if (member == null)
        {
            return null;
        }

        var userId = member.User.Id;
        var isAssigned = await _context.Equipes.AnyAsync(e => e.Id == equipeId && e.Membres.Any(u => u.Id == userId));
        if (!isAssigned)
        {
            return null;
        }

        return new MemberEquipe(memberId, equipeId) { Member = member };
    }

    public async Task<IEnumerable<Guid>> GetEquipeIdsForMemberAsync(Guid memberId)
    {
        var member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == memberId);
        if (member == null)
        {
            return [];
        }

        var userId = member.User.Id;

        return await _context.Equipes
            .Where(e => e.Membres.Any(u => u.Id == userId))
            .Select(e => e.Id)
            .ToListAsync();
    }

    public async Task ReplaceMemberEquipesAsync(Guid memberId, IEnumerable<Guid> equipeIds)
    {
        var member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == memberId);
        if (member == null)
        {
            return;
        }

        var user = member.User;

        var distinctIds = equipeIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var currentEquipes = await _context.Equipes
            .Include(e => e.Membres)
            .Where(e => e.Membres.Any(u => u.Id == user.Id))
            .ToListAsync();

        foreach (var equipe in currentEquipes)
        {
            var existingUser = equipe.Membres.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                equipe.Membres.Remove(existingUser);
            }
        }

        if (distinctIds.Count > 0)
        {
            var equipesToAssign = await _context.Equipes
                .Include(e => e.Membres)
                .Where(e => distinctIds.Contains(e.Id))
                .ToListAsync();

            foreach (var equipe in equipesToAssign)
            {
                if (equipe.Membres.All(u => u.Id != user.Id))
                {
                    equipe.Membres.Add(user);
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}

