using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Equipe;

public class EquipeNoteRepository : IEquipeNoteRepository
{
    private readonly GarneauTemplateDbContext _context;

    public EquipeNoteRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EquipeNote>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EquipeNotes
            .Include(n => n.Equipe)
            .Include(n => n.CreatedByAdmin)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EquipeNote>> GetByEquipeIdAsync(Guid equipeId, CancellationToken cancellationToken = default)
    {
        return await _context.EquipeNotes
            .Where(n => n.EquipeId == equipeId)
            .Include(n => n.Equipe)
            .Include(n => n.CreatedByAdmin)
            .ToListAsync(cancellationToken);
    }

    public async Task<EquipeNote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.EquipeNotes
            .Include(n => n.Equipe)
            .Include(n => n.CreatedByAdmin)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task AddAsync(EquipeNote note, CancellationToken cancellationToken = default)
    {
        _context.EquipeNotes.Add(note);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(EquipeNote note, CancellationToken cancellationToken = default)
    {
        _context.EquipeNotes.Update(note);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(EquipeNote note, CancellationToken cancellationToken = default)
    {
        _context.EquipeNotes.Remove(note);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
