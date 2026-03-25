using Microsoft.AspNetCore.Http;

public interface IEditEquipeRequest
{
    public string Id { get; set; }
    public string NameFr { get; set; }
    public string NameEn { get; set; }
}
