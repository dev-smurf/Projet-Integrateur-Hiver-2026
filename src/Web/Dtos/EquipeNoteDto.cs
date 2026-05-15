namespace Web.Dtos;

public class EquipeNoteDto
{
    public Guid Id { get; set; }
    public Guid EquipeId { get; set; }
    public string EquipeName { get; set; } = string.Empty;
    public Guid CreatedByAdminId { get; set; }
    public string CreatedByAdminName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public DateTime Created { get; set; }
}
