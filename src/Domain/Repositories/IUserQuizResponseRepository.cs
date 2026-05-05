using Domain.Entities;

namespace Domain.Repositories;

public interface IUserQuizResponseRepository
{
    Task<UserQuizResponse> GetByIdAsync(Guid id);
    Task<List<UserQuizResponse>> GetByUserIdAndQuizAsync(Guid userId, Guid quizId);
    Task<List<UserQuizResponse>> GetByAssignmentAsync(Guid userId, Guid assignmentId);
    Task<UserQuizResponse?> GetByUserAndQuestionAsync(Guid userId, Guid questionId);
    Task<UserQuizResponse?> GetByAssignmentAndQuestionAsync(Guid userId, Guid assignmentId, Guid questionId);
    Task CreateAsync(UserQuizResponse response);
    Task UpdateAsync(UserQuizResponse response);
    Task DeleteAsync(Guid id);
    IQueryable<UserQuizResponse> GetQueryable();
}
