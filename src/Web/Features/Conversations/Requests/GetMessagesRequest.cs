namespace Web.Features.Conversations.Requests;

public class GetMessagesRequest
{
    public Guid ConversationId { get; set; }
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 30;
}
