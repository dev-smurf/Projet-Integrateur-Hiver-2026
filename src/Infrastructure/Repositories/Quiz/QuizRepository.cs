using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;
using QuizEntity = Domain.Entities.Quiz;

namespace Infrastructure.Repositories.Quiz;

public class QuizRepository : IQuizRepository
{
    private readonly GarneauTemplateDbContext _context;

    public QuizRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public List<QuizEntity> FindAll()
    {
        return _context.Quizz
            .AsNoTracking()
            .Include(x => x.Questions)
            .ThenInclude(q => q.Responses)
            .OrderByDescending(x => x.Created)
            .ToList();
    }

    public QuizEntity FindById(Guid id)
    {
        var quiz = _context.Quizz
            .Include(x => x.Questions)
            .ThenInclude(q => q.Responses)
            .FirstOrDefault(x => x.Id == id);
        if (quiz == null)
            throw new KeyNotFoundException($"No quiz with id {id} was found.");
        return quiz;
    }

    public async Task Create(QuizEntity quiz)
    {
        _context.Quizz.Add(quiz);
        await _context.SaveChangesAsync();
    }

    public async Task Update(QuizEntity quiz)
    {
        _context.Quizz.Update(quiz);
        await _context.SaveChangesAsync();
    }
}
