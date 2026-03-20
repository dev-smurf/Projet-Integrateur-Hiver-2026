namespace Web.Features.Admins.Members.UpdateMemberModuleProgress;

public class UpdateMemberModuleProgressRequest
{
    public Guid MemberId { get; set; }
    public Guid ModuleId { get; set; }
    public int ProgressPercent { get; set; }
}
