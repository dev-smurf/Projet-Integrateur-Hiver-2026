namespace Web.Features.Admins.Notes.CreateNote;

public class CreateNoteRequest
{
    public Guid MemberId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsPrivate { get; set; } = true;
}
