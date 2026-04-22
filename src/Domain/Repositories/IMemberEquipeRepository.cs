using Domain.Entities;

namespace Domain.Repositories;

public interface IMemberEquipeRepository
{
    Task<IEnumerable<MemberEquipe>> GetByMemberIdAsync(Guid memberId);
    Task<IEnumerable<MemberEquipe>> GetByEquipeIdAsync(Guid equipeId);
    Task<MemberEquipe> AssignAsync(MemberEquipe memberEquipe);
    Task UnassignAsync(MemberEquipe memberEquipe);
    Task<bool> IsAssignedAsync(Guid memberId, Guid equipeId);
    Task<MemberEquipe?> GetByMemberAndEquipeAsync(Guid memberId, Guid equipeId);
    Task<List<Guid>> GetEquipeIdsForMemberAsync(Guid memberId);
    Task ReplaceMemberEquipesAsync(Guid memberId, IEnumerable<Guid> equipeIds);
}