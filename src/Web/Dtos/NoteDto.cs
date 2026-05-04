namespace Web.Dtos;

public class NoteDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public Guid CreatedByAdminId { get; set; }
    public string CreatedByAdminName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public DateTime Created { get; set; }
}
