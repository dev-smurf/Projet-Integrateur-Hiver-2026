namespace Domain.Repositories
{
    public interface IMemberEquipeRepository
    {
        Task AssignAsync(Domain.Entities.MemberEquipe memberEquipe);
        Task UnassignAsync(Domain.Entities.MemberEquipe memberEquipe);
        Task<IEnumerable<Domain.Entities.MemberEquipe>> GetByEquipeIdAsync(Guid equipeId);
        Task<IEnumerable<Domain.Entities.MemberEquipe>> GetByMemberIdAsync(Guid memberId);
        Task<Domain.Entities.MemberEquipe?> GetByMemberAndEquipeAsync(Guid memberId, Guid equipeId);
        Task<IEnumerable<Guid>> GetEquipeIdsForMemberAsync(Guid memberId);
        Task ReplaceMemberEquipesAsync(Guid memberId, IEnumerable<Guid> equipeIds);
    }
}
