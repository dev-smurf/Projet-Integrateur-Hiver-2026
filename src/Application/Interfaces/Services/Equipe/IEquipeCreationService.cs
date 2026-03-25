namespace Application.Interfaces.Services.Equipe;

using Domain.Entities;
using Microsoft.AspNetCore.Http;


public interface IEquipeCreationService
{
    Task CreateEquipe(Equipe equipe);
}