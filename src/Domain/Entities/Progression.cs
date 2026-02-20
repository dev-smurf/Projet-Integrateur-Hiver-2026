using Domain.Common;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Progression : AuditableAndSoftDeletableEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public int Niveau { get; private set; }

        private Progression() { }

        public Progression(Guid userId, int niveau)
        {
            UserId = userId;
            Niveau = niveau;
        }

        public void Augmenter(int valeur = 1)
        {
            Niveau += valeur;
        }
    }
}
