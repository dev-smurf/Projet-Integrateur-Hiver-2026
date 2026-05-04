using Domain.Entities;

namespace Domain.Repositories;

public interface IMemberNoteRepository
{
    Task<IEnumerable<MemberNote>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MemberNote>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<MemberNote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(MemberNote note, CancellationToken cancellationToken = default);
    Task UpdateAsync(MemberNote note, CancellationToken cancellationToken = default);
    Task DeleteAsync(MemberNote note, CancellationToken cancellationToken = default);
}
