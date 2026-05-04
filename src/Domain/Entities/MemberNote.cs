using Domain.Common;

namespace Domain.Entities;

public class MemberNote : AuditableAndSoftDeletableEntity
{
    public Guid MemberId { get; private set; }
    public Member Member { get; private set; } = null!;

    public Guid CreatedByAdminId { get; private set; }
    public Administrator CreatedByAdmin { get; private set; } = null!;

    public string Content { get; private set; } = null!;
    
    // Si True, seul l'admin peut voir. Si False, le membre peut aussi la voir.
    public bool IsPrivate { get; private set; } = true;

    private MemberNote() { }

    public MemberNote(Guid memberId, Guid createdByAdminId, string content, bool isPrivate = true)
    {
        MemberId = memberId;
        CreatedByAdminId = createdByAdminId;
        Content = content;
        IsPrivate = isPrivate;
    }

    public void UpdateContent(string content)
    {
        Content = content;
    }

    public void SetPrivacy(bool isPrivate)
    {
        IsPrivate = isPrivate;
    }
}
