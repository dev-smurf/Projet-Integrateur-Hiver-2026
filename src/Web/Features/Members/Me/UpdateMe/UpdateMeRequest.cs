using Web.Features.Common;

namespace Web.Features.Members.Me.UpdateMe;

public class UpdateMeRequest : ISanitizable
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public int? PhoneExtension { get; set; }
    public int? Apartment { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }

    public void Sanitize()
    {
        FirstName = FirstName.Trim();
        LastName = LastName.Trim();
        PhoneNumber = PhoneNumber?.Trim();
        Street = Street?.Trim();
        City = City?.Trim();
        ZipCode = ZipCode?.Trim();
    }
}
