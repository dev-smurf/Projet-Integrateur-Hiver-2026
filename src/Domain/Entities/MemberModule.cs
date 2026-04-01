using Domain.Common;

namespace Domain.Entities;

public class MemberModule : AuditableAndSoftDeletableEntity
{
    public Guid MemberId { get; private set; }
    public Member Member { get; private set; } = null!;

    public Guid ModuleId { get; private set; }
    public Module Module { get; private set; } = null!;

    public ICollection<MemberModuleSectionProgress> SectionProgress { get; private set; } = new List<MemberModuleSectionProgress>();

    public int ProgressPercent { get; private set; }
    public bool IsCompleted { get; private set; }

    private MemberModule() { }

    public MemberModule(Guid memberId, Guid moduleId)
    {
        MemberId = memberId;
        ModuleId = moduleId;
        ProgressPercent = 0;
        IsCompleted = false;
    }

    public void UpdateProgress(int progressPercent)
    {
        if (progressPercent < 0)
            progressPercent = 0;
        if (progressPercent > 100)
            progressPercent = 100;

        ProgressPercent = progressPercent;
        IsCompleted = ProgressPercent >= 100;
    }
}
