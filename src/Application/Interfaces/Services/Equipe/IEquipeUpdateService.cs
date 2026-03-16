namespace Application.Interfaces.Services.Equipe;

using Domain.Entities;
using Microsoft.AspNetCore.Http;


public interface IEquipeUpdateService
{
    Task UpdateEquipe(Equipe equipe);
}