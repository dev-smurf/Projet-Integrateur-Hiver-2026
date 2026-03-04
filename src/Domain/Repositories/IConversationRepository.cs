namespace Domain.Repositories;

using Domain.Entities;

public interface IConversationRepository
{
    Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId, bool isAdmin);
    Task<Conversation?> GetByIdAsync(Guid id);
    Task<Conversation?> GetByAdminAndMemberAsync(Guid adminId, Guid memberId);
    Task<Conversation> CreateAsync(Conversation conversation);
    Task UpdateLastMessageAtAsync(Guid conversationId, DateTime timestamp);
    Task<IEnumerable<Message>> GetMessagesAsync(Guid conversationId, int page, int pageSize);
    Task<Message> AddMessageAsync(Message message);
    Task MarkMessagesAsReadAsync(Guid conversationId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<Dictionary<Guid, int>> GetUnreadCountsPerConversationAsync(Guid userId);
}
