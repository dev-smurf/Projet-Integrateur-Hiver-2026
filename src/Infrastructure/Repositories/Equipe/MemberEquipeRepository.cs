using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Equipe;

public class MemberEquipeRepository : IMemberEquipeRepository
{
    private readonly GarneauTemplateDbContext _context;

    public MemberEquipeRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MemberEquipe>> GetByMemberIdAsync(Guid memberId)
    {
        return await _context.MemberEquipes
            .Include(me => me.Equipe)
                .ThenInclude(e => e.ParentEquipe)
            .Where(me => me.MemberId == memberId && me.Deleted == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<MemberEquipe>> GetByEquipeIdAsync(Guid equipeId)
    {
        return await _context.MemberEquipes
            .Include(me => me.Member)
                .ThenInclude(m => m.User)
            .Where(me => me.EquipeId == equipeId && me.Deleted == null)
            .ToListAsync();
    }

    public async Task<MemberEquipe> AssignAsync(MemberEquipe memberEquipe)
    {
        // Restore soft-deleted assignment if it exists
        var existing = await _context.MemberEquipes
            .FirstOrDefaultAsync(me => me.MemberId == memberEquipe.MemberId
                && me.EquipeId == memberEquipe.EquipeId
                && me.Deleted != null);

        if (existing != null)
        {
            existing.Restore();
            _context.MemberEquipes.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        _context.MemberEquipes.Add(memberEquipe);
        await _context.SaveChangesAsync();
        return memberEquipe;
    }

    public async Task UnassignAsync(MemberEquipe memberEquipe)
    {
        memberEquipe.SoftDelete();
        _context.MemberEquipes.Update(memberEquipe);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsAssignedAsync(Guid memberId, Guid equipeId)
    {
        return await _context.MemberEquipes
            .AnyAsync(me => me.MemberId == memberId && me.EquipeId == equipeId && me.Deleted == null);
    }

    public async Task<MemberEquipe?> GetByMemberAndEquipeAsync(Guid memberId, Guid equipeId)
    {
        return await _context.MemberEquipes
            .FirstOrDefaultAsync(me => me.MemberId == memberId && me.EquipeId == equipeId && me.Deleted == null);
    }

    public async Task<List<Guid>> GetEquipeIdsForMemberAsync(Guid memberId)
    {
        return await _context.MemberEquipes
            .Where(me => me.MemberId == memberId && me.Deleted == null)
            .Select(me => me.EquipeId)
            .ToListAsync();
    }

    public async Task ReplaceMemberEquipesAsync(Guid memberId, IEnumerable<Guid> equipeIds)
    {
        var distinctIds = equipeIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var currentAssignments = await _context.MemberEquipes
            .Where(me => me.MemberId == memberId && me.Deleted == null)
            .ToListAsync();

        _context.MemberEquipes.RemoveRange(currentAssignments);

        foreach (var equipeId in distinctIds)
        {
            _context.MemberEquipes.Add(new MemberEquipe(memberId, equipeId));
        }

        await _context.SaveChangesAsync();
    }
}
