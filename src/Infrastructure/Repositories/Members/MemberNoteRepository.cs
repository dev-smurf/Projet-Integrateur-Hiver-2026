using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Members;

public class MemberNoteRepository : IMemberNoteRepository
{
    private readonly GarneauTemplateDbContext _context;

    public MemberNoteRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MemberNote>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MemberNotes
            .Include(n => n.Member)
                .ThenInclude(m => m.User)
            .Include(n => n.CreatedByAdmin)
            .OrderByDescending(n => n.Created)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MemberNote>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _context.MemberNotes
            .Include(n => n.CreatedByAdmin)
            .Where(n => n.MemberId == memberId)
            .OrderByDescending(n => n.Created)
            .ToListAsync(cancellationToken);
    }

    public async Task<MemberNote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MemberNotes
            .Include(n => n.Member)
            .Include(n => n.CreatedByAdmin)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task AddAsync(MemberNote note, CancellationToken cancellationToken = default)
    {
        _context.MemberNotes.Add(note);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MemberNote note, CancellationToken cancellationToken = default)
    {
        _context.MemberNotes.Update(note);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MemberNote note, CancellationToken cancellationToken = default)
    {
        _context.MemberNotes.Remove(note);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
