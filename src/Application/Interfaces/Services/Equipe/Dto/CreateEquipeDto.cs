using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services.Equipe.Dto;

public class CreateEquipeDto
{
    public string NameFr { get; set; } = null!;
    public string NameEn { get; set; } = null!;

}
