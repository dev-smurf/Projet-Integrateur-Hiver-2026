namespace Web.Features.Admins.Members.GetRecentMembers;

public class GetRecentMembersRequest
{
    public int Days { get; set; } = 30;
    public int PageSize { get; set; } = 50;
    public string? SearchValue { get; set; }
}
