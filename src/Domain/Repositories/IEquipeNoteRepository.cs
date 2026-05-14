using Domain.Entities;

namespace Domain.Repositories;

public interface IEquipeNoteRepository
{
    Task<IEnumerable<EquipeNote>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<EquipeNote>> GetByEquipeIdAsync(Guid equipeId, CancellationToken cancellationToken = default);
    Task<EquipeNote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(EquipeNote note, CancellationToken cancellationToken = default);
    Task UpdateAsync(EquipeNote note, CancellationToken cancellationToken = default);
    Task DeleteAsync(EquipeNote note, CancellationToken cancellationToken = default);
}
