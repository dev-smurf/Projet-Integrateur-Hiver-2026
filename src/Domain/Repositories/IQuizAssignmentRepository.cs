using Domain.Entities;

namespace Domain.Repositories;

public interface IQuizAssignmentRepository
{
    Task<QuizAssignment> GetByIdAsync(Guid id);
    Task<List<QuizAssignment>> GetByUserIdAsync(Guid userId);
    Task<List<QuizAssignment>> GetByQuizIdAsync(Guid quizId);
    Task<QuizAssignment?> GetByUserIdAndQuizAsync(Guid userId, Guid quizId);
    Task<QuizAssignment?> GetAvailableByIdForUserAsync(Guid assignmentId, Guid userId);
    Task<Dictionary<Guid, int>> GetNextVersionsAsync(Guid quizId, IEnumerable<Guid> userIds);
    Task CreateAsync(QuizAssignment assignment);
    Task CreateRangeAsync(IEnumerable<QuizAssignment> assignments);
    Task UpdateAsync(QuizAssignment assignment);
    Task DeleteAsync(Guid id);
    IQueryable<QuizAssignment> GetQueryable();
}
