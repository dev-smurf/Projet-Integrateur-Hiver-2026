namespace Web.Features.Admins.Notes.UpdateNote;

public class UpdateNoteRequest
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
}
