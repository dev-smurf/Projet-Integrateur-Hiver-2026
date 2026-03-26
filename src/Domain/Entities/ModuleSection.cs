using Domain.Common;

namespace Domain.Entities;

public class ModuleSection : AuditableAndSoftDeletableEntity
{
    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public int SortOrder { get; set; }
}
