using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Conversations;

public class EquipeConversationRepository : IEquipeConversationRepository
{
    private readonly GarneauTemplateDbContext _context;

    public EquipeConversationRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<EquipeConversation?> GetByIdAsync(Guid id)
    {
        return await _context.EquipeConversations
            .Include(c => c.Equipe)
            .FirstOrDefaultAsync(c => c.Id == id && c.Deleted == null);
    }

    public async Task<EquipeConversation?> GetByEquipeIdAsync(Guid equipeId)
    {
        return await _context.EquipeConversations
            .Include(c => c.Equipe)
            .FirstOrDefaultAsync(c => c.EquipeId == equipeId && c.Deleted == null);
    }

    public async Task<IEnumerable<EquipeConversation>> GetByUserIdAsync(Guid userId, bool isAdmin)
    {
        var query = _context.EquipeConversations
            .Include(c => c.Equipe)
                .ThenInclude(e => e.Membres)
            .Include(c => c.Messages.OrderByDescending(m => m.Date).Take(1))
            .Where(c => c.Deleted == null && c.Equipe.Deleted == null);

        if (!isAdmin)
        {
            query = query.Where(c => c.Equipe.Membres.Any(u => u.Id == userId));
        }

        return await query.OrderByDescending(c => c.LastMessageAt).ToListAsync();
    }

    public async Task<EquipeConversation> EnsureConversationForEquipeAsync(Guid equipeId)
    {
        var existing = await _context.EquipeConversations
            .FirstOrDefaultAsync(c => c.EquipeId == equipeId && c.Deleted == null);

        if (existing != null)
            return existing;

        var conversation = new EquipeConversation
        {
            EquipeId = equipeId,
            LastMessageAt = DateTime.UtcNow
        };

        _context.EquipeConversations.Add(conversation);
        await _context.SaveChangesAsync();
        return conversation;
    }

    public async Task EnsureConversationsForAdminAsync()
    {
        var existingEquipeIds = await _context.EquipeConversations
            .Where(c => c.Deleted == null)
            .Select(c => c.EquipeId)
            .ToListAsync();

        var allEquipeIds = await _context.Equipes
            .Where(e => e.Deleted == null)
            .Select(e => e.Id)
            .ToListAsync();

        var missingEquipeIds = allEquipeIds.Except(existingEquipeIds).ToList();

        if (missingEquipeIds.Count == 0) return;

        foreach (var equipeId in missingEquipeIds)
        {
            _context.EquipeConversations.Add(new EquipeConversation
            {
                EquipeId = equipeId,
                LastMessageAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<EquipeMessage> AddMessageAsync(EquipeMessage message)
    {
        _context.EquipeMessages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<IEnumerable<EquipeMessage>> GetMessagesAsync(Guid conversationId, int page, int pageSize)
    {
        return await _context.EquipeMessages
            .Include(m => m.Expediteur)
            .Where(m => m.EquipeConversationId == conversationId && m.Deleted == null)
            .OrderByDescending(m => m.Date)
            .Skip(page * pageSize)
            .Take(pageSize)
            .OrderBy(m => m.Date)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, string>> GetSenderNamesByUserIdsAsync(IEnumerable<Guid> userIds)
    {
        var ids = userIds.Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<Guid, string>();

        var memberNames = await _context.Members
            .Where(m => ids.Contains(m.User.Id))
            .ToDictionaryAsync(m => m.User.Id, m => m.FirstName + " " + m.LastName);

        var adminNames = await _context.Administrators
            .Where(a => ids.Contains(a.User.Id))
            .ToDictionaryAsync(a => a.User.Id, a => a.FirstName + " " + a.LastName);

        foreach (var kvp in adminNames)
        {
            memberNames[kvp.Key] = kvp.Value;
        }

        return memberNames;
    }

    public async Task MarkAsReadAsync(Guid conversationId, Guid userId)
    {
        var readMessageIds = await _context.EquipeMessageReads
            .Where(r => r.UserId == userId
                        && _context.EquipeMessages
                            .Where(m => m.EquipeConversationId == conversationId)
                            .Select(m => m.Id)
                            .Contains(r.EquipeMessageId))
            .Select(r => r.EquipeMessageId)
            .ToListAsync();

        var unreadMessages = await _context.EquipeMessages
            .Where(m => m.EquipeConversationId == conversationId
                        && m.Deleted == null
                        && m.ExpediteurId != userId
                        && !readMessageIds.Contains(m.Id))
            .Select(m => m.Id)
            .ToListAsync();

        if (unreadMessages.Count == 0) return;

        var now = DateTime.UtcNow;
        foreach (var msgId in unreadMessages)
        {
            _context.EquipeMessageReads.Add(new EquipeMessageRead
            {
                EquipeMessageId = msgId,
                UserId = userId,
                ReadAt = now
            });
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyError(ex))
        {
            // Race with a concurrent mark-as-read for the same (message, user) pair.
            // The desired state is achieved: ignore and continue.
        }
    }

    private static bool IsDuplicateKeyError(DbUpdateException ex)
    {
        // SQL Server: 2627 (unique constraint) or 2601 (unique index).
        if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
        {
            return sqlEx.Number == 2627 || sqlEx.Number == 2601;
        }
        return false;
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        // Messages the user can see = messages in equipes the user belongs to (or all for admin),
        // not sent by the user, and not yet read by the user.
        var isAdmin = await _context.Administrators.AnyAsync(a => a.User.Id == userId);

        var accessibleConversationIdsQuery = _context.EquipeConversations
            .Where(c => c.Deleted == null && c.Equipe.Deleted == null);

        if (!isAdmin)
        {
            accessibleConversationIdsQuery = accessibleConversationIdsQuery
                .Where(c => c.Equipe.Membres.Any(u => u.Id == userId));
        }

        var accessibleConversationIds = await accessibleConversationIdsQuery
            .Select(c => c.Id)
            .ToListAsync();

        if (accessibleConversationIds.Count == 0) return 0;

        return await _context.EquipeMessages
            .AsNoTracking()
            .Where(m => m.Deleted == null
                        && accessibleConversationIds.Contains(m.EquipeConversationId)
                        && m.ExpediteurId != userId
                        && !m.Reads.Any(r => r.UserId == userId))
            .CountAsync();
    }

    public async Task<Dictionary<Guid, int>> GetUnreadCountsPerConversationAsync(Guid userId)
    {
        var isAdmin = await _context.Administrators.AnyAsync(a => a.User.Id == userId);

        var accessibleConversationIdsQuery = _context.EquipeConversations
            .Where(c => c.Deleted == null && c.Equipe.Deleted == null);

        if (!isAdmin)
        {
            accessibleConversationIdsQuery = accessibleConversationIdsQuery
                .Where(c => c.Equipe.Membres.Any(u => u.Id == userId));
        }

        var accessibleConversationIds = await accessibleConversationIdsQuery
            .Select(c => c.Id)
            .ToListAsync();

        if (accessibleConversationIds.Count == 0) return new Dictionary<Guid, int>();

        return await _context.EquipeMessages
            .Where(m => m.Deleted == null
                        && accessibleConversationIds.Contains(m.EquipeConversationId)
                        && m.ExpediteurId != userId
                        && !m.Reads.Any(r => r.UserId == userId))
            .GroupBy(m => m.EquipeConversationId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<IEnumerable<Guid>> GetMemberUserIdsForEquipeAsync(Guid equipeId)
    {
        return await _context.Equipes
            .Where(e => e.Id == equipeId && e.Deleted == null)
            .SelectMany(e => e.Membres.Select(u => u.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetEquipeIdsForUserAsync(Guid userId, bool isAdmin)
    {
        if (isAdmin)
        {
            return await _context.Equipes
                .Where(e => e.Deleted == null)
                .Select(e => e.Id)
                .ToListAsync();
        }

        return await _context.Equipes
            .Where(e => e.Deleted == null && e.Membres.Any(m => m.Id == userId))
            .Select(e => e.Id)
            .ToListAsync();
    }

    public async Task<bool> IsUserInEquipeAsync(Guid userId, Guid equipeId, bool isAdmin)
    {
        if (isAdmin)
        {
            return await _context.Equipes.AnyAsync(e => e.Id == equipeId && e.Deleted == null);
        }

        return await _context.Equipes
            .AnyAsync(e => e.Id == equipeId && e.Deleted == null && e.Membres.Any(m => m.Id == userId));
    }

    public async Task UpdateLastMessageAtAsync(Guid conversationId, DateTime date)
    {
        var conversation = await _context.EquipeConversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.LastMessageAt = date;
            await _context.SaveChangesAsync();
        }
    }
}
