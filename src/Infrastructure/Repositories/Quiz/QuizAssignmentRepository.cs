using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Quiz;

public class QuizAssignmentRepository : IQuizAssignmentRepository
{
    private readonly GarneauTemplateDbContext _context;

    public QuizAssignmentRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<QuizAssignment> GetByIdAsync(Guid id)
    {
        var assignment = await _context.QuizAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (assignment == null)
            throw new KeyNotFoundException($"No quiz assignment with id {id} was found.");

        return assignment;
    }

    public async Task<List<QuizAssignment>> GetByUserIdAsync(Guid userId)
    {
        return await _context.QuizAssignments
            .AsNoTracking()
            .Include(x => x.Quiz)
            .Where(x => x.UserId == userId)
            .Where(x => !x.AvailableAt.HasValue || x.AvailableAt <= DateTime.UtcNow)
            .OrderByDescending(x => x.AssignedAt)
            .ThenByDescending(x => x.Version)
            .ToListAsync();
    }

    public async Task<List<QuizAssignment>> GetByQuizIdAsync(Guid quizId)
    {
        return await _context.QuizAssignments
            .AsNoTracking()
            .Where(x => x.QuizId == quizId)
            .OrderBy(x => x.AssignedAt)
            .ToListAsync();
    }

    public async Task<QuizAssignment?> GetByUserIdAndQuizAsync(Guid userId, Guid quizId)
    {
        return await _context.QuizAssignments
            .OrderByDescending(x => x.AssignedAt)
            .ThenByDescending(x => x.Version)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.QuizId == quizId);
    }

    public async Task<QuizAssignment?> GetAvailableByIdForUserAsync(Guid assignmentId, Guid userId)
    {
        return await _context.QuizAssignments
            .Include(x => x.Quiz)
            .ThenInclude(x => x.Questions)
            .ThenInclude(x => x.Responses)
            .FirstOrDefaultAsync(x =>
                x.Id == assignmentId
                && x.UserId == userId
                && (!x.AvailableAt.HasValue || x.AvailableAt <= DateTime.UtcNow));
    }

    public async Task<Dictionary<Guid, int>> GetNextVersionsAsync(Guid quizId, IEnumerable<Guid> userIds)
    {
        var userIdList = userIds.Distinct().ToList();
        var latestVersions = await _context.QuizAssignments
            .AsNoTracking()
            .Where(x => x.QuizId == quizId && userIdList.Contains(x.UserId))
            .GroupBy(x => x.UserId)
            .Select(x => new { UserId = x.Key, Version = x.Max(a => a.Version) })
            .ToListAsync();

        var versionByUserId = latestVersions.ToDictionary(x => x.UserId, x => x.Version + 1);

        foreach (var userId in userIdList.Where(x => !versionByUserId.ContainsKey(x)))
            versionByUserId[userId] = 1;

        return versionByUserId;
    }

    public async Task CreateAsync(QuizAssignment assignment)
    {
        _context.QuizAssignments.Add(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task CreateRangeAsync(IEnumerable<QuizAssignment> assignments)
    {
        _context.QuizAssignments.AddRange(assignments);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(QuizAssignment assignment)
    {
        _context.QuizAssignments.Update(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var assignment = await GetByIdAsync(id);
        _context.QuizAssignments.Remove(assignment);
        await _context.SaveChangesAsync();
    }

    public IQueryable<QuizAssignment> GetQueryable()
    {
        return _context.QuizAssignments.AsNoTracking();
    }
}
