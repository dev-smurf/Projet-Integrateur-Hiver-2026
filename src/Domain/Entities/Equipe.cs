using Domain.Common;
using Domain.Entities.Identity;
using Domain.Extensions;

namespace Domain.Entities
{
    public class Equipe : AuditableAndSoftDeletableEntity
    {
        public string NameFr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        public Guid? ParentEquipeId { get; private set; }
        public Equipe? ParentEquipe { get; private set; }
        public ICollection<Equipe> SousEquipes { get; private set; } = new List<Equipe>();
        public ICollection<User> Membres { get; private set; } = new List<User>();

        public void SanitazeForSaving()
        {
            NameFr = NameFr.Trim().CapitalizeFirstLetterOfEachWord()!;
            NameEn = NameEn.Trim().CapitalizeFirstLetterOfEachWord()!;
        }

        public void SetParentEquipe(Guid? parentEquipeId)
        {
            ParentEquipeId = parentEquipeId;
        }

        public Equipe() { }

        public Equipe(string nameFr, string nameEn, string? parentEquipeId = null)
        {
            NameFr = nameFr;
            NameEn = nameEn;
            ParentEquipeId = Guid.TryParse(parentEquipeId, out var guid) ? guid : null;
        }
    }
}
