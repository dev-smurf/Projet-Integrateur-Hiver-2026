namespace Infrastructure.Mailing;

public class MailingSettings
{
    public Dictionary<string, string> FromName { get; set; } = null!;
    public Dictionary<string, string> FromAddress { get; set; } = null!;
    public string ToAddressForDevelopment { get; set; } = null!;
}