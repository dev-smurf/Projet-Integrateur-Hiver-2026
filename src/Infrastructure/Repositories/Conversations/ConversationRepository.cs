using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Conversations;

public class ConversationRepository : IConversationRepository
{
    private readonly GarneauTemplateDbContext _context;

    public ConversationRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId, bool isAdmin)
    {
        var query = _context.Conversations
            .Include(c => c.Admin)
            .Include(c => c.Member)
            .Include(c => c.Messages.OrderByDescending(m => m.Date).Take(1))
            .Where(c => c.Deleted == null);

        query = isAdmin
            ? query.Where(c => c.AdminId == userId)
            : query.Where(c => c.MemberId == userId);

        return await query.OrderByDescending(c => c.LastMessageAt).ToListAsync();
    }

    public async Task<Conversation?> GetByIdAsync(Guid id)
    {
        return await _context.Conversations
            .Include(c => c.Admin)
            .Include(c => c.Member)
            .FirstOrDefaultAsync(c => c.Id == id && c.Deleted == null);
    }

    public async Task<Conversation?> GetByAdminAndMemberAsync(Guid adminId, Guid memberId)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(c => c.AdminId == adminId && c.MemberId == memberId && c.Deleted == null);
    }

    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
        return conversation;
    }

    public async Task UpdateLastMessageAtAsync(Guid conversationId, DateTime timestamp)
    {
        var conversation = await _context.Conversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.LastMessageAt = timestamp;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId, int page, int pageSize)
    {
        return await _context.Messages
            .Include(m => m.Expediteur)
            .Where(m => m.ConversationId == conversationId && m.Deleted == null)
            .OrderByDescending(m => m.Date)
            .Skip(page * pageSize)
            .Take(pageSize)
            .OrderBy(m => m.Date)
            .ToListAsync();
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId)
    {
        var unreadMessages = await _context.Messages
            .Where(m => m.ConversationId == conversationId
                        && m.ReceveurId == userId
                        && m.ReadAt == null
                        && m.Deleted == null)
            .ToListAsync();

        foreach (var msg in unreadMessages)
            msg.MarkAsRead();

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Messages
            .Where(m => m.ReceveurId == userId && m.ReadAt == null && m.Deleted == null)
            .CountAsync();
    }

    public async Task<Dictionary<Guid, int>> GetUnreadCountsPerConversationAsync(Guid userId)
    {
        return await _context.Messages
            .Where(m => m.ReceveurId == userId && m.ReadAt == null && m.Deleted == null)
            .GroupBy(m => m.ConversationId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }
}
