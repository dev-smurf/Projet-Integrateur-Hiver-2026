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

    public async Task<List<MemberModuleSectionProgress>> GetSectionProgressAsync(Guid memberModuleId)
    {
        return await _context.MemberModuleSectionProgress
            .Where(sp => sp.MemberModuleId == memberModuleId && sp.Deleted == null)
            .ToListAsync();
    }

    public async Task MarkSectionReadAsync(Guid memberId, Guid moduleId, Guid sectionId)
    {
        var memberModule = await _context.MemberModules
            .Include(mm => mm.SectionProgress.Where(sp => sp.Deleted == null))
            .FirstOrDefaultAsync(mm => mm.MemberId == memberId && mm.ModuleId == moduleId && mm.Deleted == null);

        if (memberModule == null) return;

        var existing = memberModule.SectionProgress.FirstOrDefault(sp => sp.ModuleSectionId == sectionId);
        if (existing != null)
        {
            existing.MarkAsRead();
        }
        else
        {
            var progress = new MemberModuleSectionProgress(memberModule.Id, sectionId);
            progress.MarkAsRead();
            _context.MemberModuleSectionProgress.Add(progress);
        }

        // Recalculate module progress
        var totalSections = await _context.ModuleSections
            .CountAsync(s => s.ModuleId == moduleId && s.Deleted == null);

        if (totalSections > 0)
        {
            var readSections = memberModule.SectionProgress.Count(sp => sp.IsRead) +
                (existing == null ? 1 : 0);
            var percent = (int)Math.Round((double)readSections / totalSections * 100);
            memberModule.UpdateProgress(percent);
        }

        await _context.SaveChangesAsync();
    }
}
