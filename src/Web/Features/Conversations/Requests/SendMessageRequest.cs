namespace Web.Features.Conversations.Requests;

public class SendMessageRequest
{
    public Guid ConversationId { get; set; }
    public string Text { get; set; } = null!;
}
