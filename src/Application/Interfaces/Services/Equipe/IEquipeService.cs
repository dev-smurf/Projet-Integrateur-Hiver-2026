using Application.Interfaces.Services.Equipe.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Equipe
{
     public interface IEquipeService
    {
        Task<List<EquipeDto>> GetAllEquipes();
        Task<bool> UpdateEquipe(string id, EquipeDto request);
        Task<EquipeDto> GetEquipe(string id);
        Task<EquipeDto> CreateEquipe(CreateEquipeDto request);
    }
}