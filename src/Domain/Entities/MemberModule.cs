using Domain.Common;

namespace Domain.Entities;

public class MemberModule : AuditableAndSoftDeletableEntity
{
    public Guid MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = null!;
}
