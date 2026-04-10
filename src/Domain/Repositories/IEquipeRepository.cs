namespace Domain.Repositories;

using Domain.Entities;


public interface IEquipeRepository
{
    Task<List<Equipe>> GetAll();
    Task<List<Equipe>> GetByIds(IEnumerable<Guid> ids);
    Task<List<Guid>> GetEquipeIdsForUser(Guid userId);
    Task AssignUserToEquipes(Domain.Entities.Identity.User user, IEnumerable<Guid> ids);
    Task ReplaceUserEquipes(Domain.Entities.Identity.User user, IEnumerable<Guid> ids);
    Task<Equipe?> FindById(Guid id);
    Task CreateEquipe(Equipe equipe);
    Task UpdateEquipe(Equipe equipe);
    Task DeleteEquipeWithId(Guid id);
}
