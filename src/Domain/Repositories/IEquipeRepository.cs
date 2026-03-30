namespace Domain.Repositories;

using Domain.Entities;

public interface IEquipeRepository
{
    Task<List<Equipe>> GetAll();
    Task<Equipe?> FindById(Guid id);
    Task<Equipe?> FindByUserId(Guid userId);
    Task CreateEquipe(Equipe equipe);
    Task UpdateEquipe(Equipe equipe);
    Task DeleteEquipeWithId(Guid id);
}