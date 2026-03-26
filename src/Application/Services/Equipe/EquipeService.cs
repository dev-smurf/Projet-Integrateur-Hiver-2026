using Application.Interfaces.Services.Equipe;
using Application.Interfaces.Services.Equipe.Dto;
using Domain.Repositories;

namespace Application.Services.Equipe
{
    public class EquipeService : IEquipeService
    {
        private readonly IEquipeRepository _equipeRepository;

        public EquipeService(IEquipeRepository equipeRepository)
        {
            _equipeRepository = equipeRepository;
        }

        public async Task<List<EquipeDto>> GetAllEquipes()
        {
            var equipes = await _equipeRepository.GetAll();

            return equipes.Select(e => new EquipeDto
            {
                Id = e.Id.ToString(),
                NameFr = e.NameFr,
                NameEn = e.NameEn,
            }).ToList();
        }

        public async Task<EquipeDto> GetEquipe(string id)
        {
            if (!Guid.TryParse(id, out Guid guidId))
                return null;

            var equipe = await _equipeRepository.FindById(guidId);
            if (equipe == null)
                return null;

            return new EquipeDto
            {
                Id = equipe.Id.ToString(),
                NameFr = equipe.NameFr,
                NameEn = equipe.NameEn
            };
        }

        public async Task<EquipeDto> CreateEquipe(CreateEquipeDto request)
        {
            var equipe = new Domain.Entities.Equipe
            {
                NameFr = request.NameFr,
                NameEn = request.NameEn,
            };

            await _equipeRepository.CreateEquipe(equipe);

            return new EquipeDto
            {
                Id = equipe.Id.ToString(),
                NameFr = equipe.NameFr,
                NameEn = equipe.NameEn,
            };
        }

        public async Task<bool> UpdateEquipe(string id, EquipeDto request)
        {
            if (!Guid.TryParse(id, out Guid guidId))
                return false;

            var existingEquipe = await _equipeRepository.FindById(guidId);
            if (existingEquipe == null)
                return false;

            existingEquipe.NameFr = request.NameFr ?? existingEquipe.NameFr;
            existingEquipe.NameEn = request.NameEn ?? existingEquipe.NameEn;

            await _equipeRepository.UpdateEquipe(existingEquipe);
            return true;
        }
    }
}
