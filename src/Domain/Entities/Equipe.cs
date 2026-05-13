using Domain.Common;
using Domain.Entities.Identity;
using Domain.Extensions;

namespace Domain.Entities
{
    public class Equipe : AuditableAndSoftDeletableEntity
    {
        public string NameFr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public Guid? ParentEquipeId { get; set; }
        public Equipe? ParentEquipe { get; set; }
        public ICollection<Equipe> ChildEquipes { get; private set; } = new List<Equipe>();

        public ICollection<User> Membres { get; private set; } = new List<User>();

        public void SanitazeForSaving()
        {
            NameFr = NameFr.Trim().CapitalizeFirstLetterOfEachWord()!;
            NameEn = NameEn.Trim().CapitalizeFirstLetterOfEachWord()!;
        }

        public Equipe() { }

        public Equipe(string nameFr, string nameEn)
        {
            NameFr = nameFr;
            NameEn = nameEn;
        }
    }
}
