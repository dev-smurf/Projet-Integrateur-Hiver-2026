using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Module;

public class MemberModuleRepository : IMemberModuleRepository
{
    private readonly GarneauTemplateDbContext _context;

    public MemberModuleRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MemberModule>> GetByMemberIdAsync(Guid memberId)
    {
        return await _context.MemberModules
            .Include(mm => mm.Module)
            .Where(mm => mm.MemberId == memberId && mm.Deleted == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<MemberModule>> GetByModuleIdAsync(Guid moduleId)
    {
        return await _context.MemberModules
            .Include(mm => mm.Member)
            .Where(mm => mm.ModuleId == moduleId && mm.Deleted == null)
            .ToListAsync();
    }

    public async Task<MemberModule> AssignAsync(MemberModule memberModule)
    {
        // Restore soft-deleted assignment if it exists
        var existing = await _context.MemberModules
            .FirstOrDefaultAsync(mm => mm.MemberId == memberModule.MemberId
                && mm.ModuleId == memberModule.ModuleId
                && mm.Deleted != null);

        if (existing != null)
        {
            existing.Restore();
            _context.MemberModules.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }

        _context.MemberModules.Add(memberModule);
        await _context.SaveChangesAsync();
        return memberModule;
    }

    public async Task UnassignAsync(MemberModule memberModule)
    {
        memberModule.SoftDelete();
        _context.MemberModules.Update(memberModule);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsAssignedAsync(Guid memberId, Guid moduleId)
    {
        return await _context.MemberModules
            .AnyAsync(mm => mm.MemberId == memberId && mm.ModuleId == moduleId && mm.Deleted == null);
    }

    public async Task<MemberModule?> GetByMemberAndModuleAsync(Guid memberId, Guid moduleId)
    {
        return await _context.MemberModules
            .FirstOrDefaultAsync(mm => mm.MemberId == memberId && mm.ModuleId == moduleId && mm.Deleted == null);
    }
}
