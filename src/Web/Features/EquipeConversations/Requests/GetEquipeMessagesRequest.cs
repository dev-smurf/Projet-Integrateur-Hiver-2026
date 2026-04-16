namespace Web.Features.EquipeConversations.Requests;

public class GetEquipeMessagesRequest
{
    public Guid ConversationId { get; set; }
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 30;
}
