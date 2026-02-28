namespace Web.Features.Admins.Module;

public class IEditModuleRequest
{
    public string Id { get; set; }

    public string? NameFr { get; set; }
    public string? ContenueFr { get; set; }
    public string? SujetFr { get; set; }
    public IFormFile? CardImage { get; set; }
}


