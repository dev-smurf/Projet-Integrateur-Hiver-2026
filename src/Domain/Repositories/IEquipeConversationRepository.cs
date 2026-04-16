using Domain.Entities;

namespace Domain.Repositories;

public interface IEquipeConversationRepository
{
    Task<EquipeConversation?> GetByIdAsync(Guid id);
    Task<EquipeConversation?> GetByEquipeIdAsync(Guid equipeId);
    Task<IEnumerable<EquipeConversation>> GetByUserIdAsync(Guid userId, bool isAdmin);

    Task<EquipeConversation> EnsureConversationForEquipeAsync(Guid equipeId);
    Task EnsureConversationsForAdminAsync();

    Task<EquipeMessage> AddMessageAsync(EquipeMessage message);
    Task<IEnumerable<EquipeMessage>> GetMessagesAsync(Guid conversationId, int page, int pageSize);
    Task<Dictionary<Guid, string>> GetSenderNamesByUserIdsAsync(IEnumerable<Guid> userIds);

    Task MarkAsReadAsync(Guid conversationId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<Dictionary<Guid, int>> GetUnreadCountsPerConversationAsync(Guid userId);

    Task<IEnumerable<Guid>> GetMemberUserIdsForEquipeAsync(Guid equipeId);
    Task<IEnumerable<Guid>> GetEquipeIdsForUserAsync(Guid userId, bool isAdmin);
    Task<bool> IsUserInEquipeAsync(Guid userId, Guid equipeId, bool isAdmin);

    Task UpdateLastMessageAtAsync(Guid conversationId, DateTime date);
}
