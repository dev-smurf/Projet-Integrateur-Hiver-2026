using Domain.Entities;

namespace Domain.Repositories;

public interface IMemberModuleRepository
{
    Task<IEnumerable<MemberModule>> GetByMemberIdAsync(Guid memberId);
    Task<IEnumerable<MemberModule>> GetByModuleIdAsync(Guid moduleId);
    Task<MemberModule> AssignAsync(MemberModule memberModule);
    Task UnassignAsync(MemberModule memberModule);
    Task<bool> IsAssignedAsync(Guid memberId, Guid moduleId);
    Task<MemberModule?> GetByMemberAndModuleAsync(Guid memberId, Guid moduleId);
}
