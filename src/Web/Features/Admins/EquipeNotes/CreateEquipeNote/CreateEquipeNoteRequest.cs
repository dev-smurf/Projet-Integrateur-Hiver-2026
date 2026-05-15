namespace Web.Features.Admins.EquipeNotes.CreateEquipeNote;

public class CreateEquipeNoteRequest
{
    public Guid EquipeId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsPrivate { get; set; } = true;
}
