namespace Web.Features.EquipeConversations.Requests;

public class SendEquipeMessageRequest
{
    public Guid ConversationId { get; set; }
    public string? Text { get; set; }
    public IFormFile? Attachment { get; set; }
}
