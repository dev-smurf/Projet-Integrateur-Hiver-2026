namespace Web.Dtos;

public class DashboardSummaryDto
{
    public int TotalMembers { get; set; }
    public int NewMembersLast30Days { get; set; }
    public int TotalModules { get; set; }
    public int TotalMemberModules { get; set; }
    public int CompletedMemberModules { get; set; }
    public int AverageProgressPercent { get; set; }
}
