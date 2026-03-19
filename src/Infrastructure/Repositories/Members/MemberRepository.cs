using Application.Exceptions.Members;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Members;

public class MemberRepository : IMemberRepository
{
    private readonly GarneauTemplateDbContext _context;

    public MemberRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public PaginatedList<Member> GetAllPaginated(int pageIndex, int pageSize, string? searchValue = null)
    {
        var query = _context.Members
            .Include(x => x.User)
            .ThenInclude(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var search = searchValue.ToLower();
            query = query.Where(m =>
                m.FirstName.ToLower().Contains(search) ||
                m.LastName.ToLower().Contains(search) ||
                m.User.Email!.ToLower().Contains(search) ||
                (m.City != null && m.City.ToLower().Contains(search)));
        }

        var totalItems = query.Count();
        var pageItems = query.OrderByDescending(x => x.Created).Skip((pageIndex-1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<Member>(pageItems, totalItems);
    }

    public Member FindById(Guid id)
    {
        var member = _context.Members
            .Include(x => x.User)
            .ThenInclude(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefault(x => x.Id == id);
        if (member == null)
            throw new MemberNotFoundException($"No member with id {id} was found.");
        return member;
    }

    public Member? FindByUserId(Guid userId, bool asNoTracking = true)
    {
        var query = _context.Members as IQueryable<Member>;
        if (asNoTracking)
            query = query.AsNoTracking();
        return query
            .Include(x => x.User)
            .ThenInclude(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefault(x => x.User.Id == userId);
    }

    public Member? FindByUserEmail(string userEmail)
    {
        return _context.Members
            .Include(x => x.User)
            .ThenInclude(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefault(x => x.User.Email == userEmail);
    }

    public async Task Create(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Member member)
    {
        if (!_context.Members.Any(x => x.Id == member.Id))
            throw new MemberNotFoundException($"Could not find member with id {member.Id}.");

        _context.Members.Update(member);
        await _context.SaveChangesAsync();
    }

    public async Task AddModuleToMember(Guid memberId, Guid moduleId)
    {
        var exists = await _context.MemberModules
            .AnyAsync(x => x.MemberId == memberId && x.ModuleId == moduleId);
        if (exists)
            return;

        _context.MemberModules.Add(new MemberModule(memberId, moduleId));
        await _context.SaveChangesAsync();
    }

    public async Task<List<MemberModule>> GetMemberModules(Guid memberId)
    {
        return await _context.MemberModules
            .Include(x => x.Module)
            .Where(x => x.MemberId == memberId)
            .OrderByDescending(x => x.Created)
            .ToListAsync();
    }

    public async Task UpdateMemberModuleProgress(Guid memberId, Guid moduleId, int progressPercent)
    {
        var memberModule = await _context.MemberModules
            .FirstOrDefaultAsync(x => x.MemberId == memberId && x.ModuleId == moduleId);
        if (memberModule == null)
            return;

        memberModule.UpdateProgress(progressPercent);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveModuleFromMember(Guid memberId, Guid moduleId)
    {
        var memberModule = await _context.MemberModules
            .FirstOrDefaultAsync(x => x.MemberId == memberId && x.ModuleId == moduleId);
        if (memberModule == null)
            return;

        _context.MemberModules.Remove(memberModule);
        await _context.SaveChangesAsync();
    }
}
