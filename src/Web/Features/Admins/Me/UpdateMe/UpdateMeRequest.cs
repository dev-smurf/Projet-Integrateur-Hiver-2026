using Web.Features.Common;

namespace Web.Features.Admins.Me.UpdateMe;

public class UpdateMeRequest : ISanitizable
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public void Sanitize()
    {
        FirstName = FirstName.Trim();
        LastName = LastName.Trim();
    }
}
