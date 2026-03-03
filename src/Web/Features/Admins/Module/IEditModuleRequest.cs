using Microsoft.AspNetCore.Http;

public interface IEditModuleRequest
{
    string? NameFr { get; set; }
    string? NameEn { get; set; }
    string? SujetFr { get; set; }
    string? SujetEn { get; set; }
    string? ContenueFr { get; set; }
    string? ContenueEn { get; set; }

    IFormFile? CardImage { get; set; }
}
