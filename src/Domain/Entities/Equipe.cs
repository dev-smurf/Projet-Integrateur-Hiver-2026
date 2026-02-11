using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Equipe : AuditableAndSoftDeletableEntity
    {
        public string Nom { get; private set; } = null!;

        public ICollection<User> Membres { get; private set; } = new List<User>();

        public void SanitazeForSaving()
        {
            Nom = Nom.Trim();
        }

        private Equipe() { }

        public Equipe(string nom)
        {
            Nom = nom;
        }
    }
}
