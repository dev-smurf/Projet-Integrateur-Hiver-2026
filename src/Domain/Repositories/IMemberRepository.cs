using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IMemberRepository
{
    PaginatedList<Member> GetAllPaginated(int pageIndex, int pageSize, string? searchValue = null);
    Member FindById(Guid id);
    Member? FindByUserId(Guid userId, bool asNoTracking = true);
    Member? FindByUserEmail(string userEmail);
    Task Create(Member member);
    Task Update(Member member);
    Task AddModuleToMember(Guid memberId, Guid moduleId);
    Task<List<MemberModule>> GetMemberModules(Guid memberId);
    Task UpdateMemberModuleProgress(Guid memberId, Guid moduleId, int progressPercent);
}
