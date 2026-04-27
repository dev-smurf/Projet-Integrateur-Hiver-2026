using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Quiz;

public class UserQuizResponseRepository : IUserQuizResponseRepository
{
    private readonly GarneauTemplateDbContext _context;

    public UserQuizResponseRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<UserQuizResponse> GetByIdAsync(Guid id)
    {
        var response = await _context.UserQuizResponses
            .AsNoTracking()
            .Include(x => x.Question)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (response == null)
            throw new KeyNotFoundException($"No user quiz response with id {id} was found.");

        return response;
    }

    public async Task<List<UserQuizResponse>> GetByUserIdAndQuizAsync(Guid userId, Guid quizId)
    {
        return await _context.UserQuizResponses
            .AsNoTracking()
            .Include(x => x.Question)
            .Where(x => x.UserId == userId && x.Question.QuizId == quizId)
            .OrderBy(x => x.Question.Order)
            .ToListAsync();
    }

    public async Task<List<UserQuizResponse>> GetByAssignmentAsync(Guid userId, Guid assignmentId)
    {
        return await _context.UserQuizResponses
            .AsNoTracking()
            .Include(x => x.Question)
            .Where(x => x.UserId == userId && x.QuizAssignmentId == assignmentId)
            .OrderBy(x => x.Question.Order)
            .ToListAsync();
    }

    public async Task<UserQuizResponse?> GetByUserAndQuestionAsync(Guid userId, Guid questionId)
    {
        return await _context.UserQuizResponses
            .AsNoTracking()
            .Include(x => x.Question)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.QuizQuestionId == questionId);
    }

    public async Task<UserQuizResponse?> GetByAssignmentAndQuestionAsync(Guid userId, Guid assignmentId, Guid questionId)
    {
        return await _context.UserQuizResponses
            .AsNoTracking()
            .Include(x => x.Question)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId
                && x.QuizAssignmentId == assignmentId
                && x.QuizQuestionId == questionId);
    }

    public async Task CreateAsync(UserQuizResponse response)
    {
        _context.UserQuizResponses.Add(response);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserQuizResponse response)
    {
        _context.UserQuizResponses.Update(response);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await GetByIdAsync(id);
        _context.UserQuizResponses.Remove(response);
        await _context.SaveChangesAsync();
    }

    public IQueryable<UserQuizResponse> GetQueryable()
    {
        return _context.UserQuizResponses.AsNoTracking();
    }
}
