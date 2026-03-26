namespace Domain.Repositories;

public interface IQuizRepository
{
    List<Domain.Entities.Quiz> FindAll();
    Domain.Entities.Quiz FindById(Guid id);
    Task Create(Domain.Entities.Quiz quiz);
    Task Update(Domain.Entities.Quiz quiz);
}
