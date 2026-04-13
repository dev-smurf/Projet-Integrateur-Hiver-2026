using Domain.Common;
using Domain.Helpers;
using NodaTime;

namespace Domain.Entities;

public class MemberModuleSectionProgress : AuditableAndSoftDeletableEntity
{
    public Guid MemberModuleId { get; private set; }
    public MemberModule MemberModule { get; private set; } = null!;

    public Guid ModuleSectionId { get; private set; }
    public ModuleSection ModuleSection { get; private set; } = null!;

    public bool IsRead { get; private set; }
    public Instant? ReadAt { get; private set; }

    private MemberModuleSectionProgress() { }

    public MemberModuleSectionProgress(Guid memberModuleId, Guid moduleSectionId)
    {
        MemberModuleId = memberModuleId;
        ModuleSectionId = moduleSectionId;
        IsRead = false;
    }

    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = InstantHelper.GetLocalNow();
    }
}
