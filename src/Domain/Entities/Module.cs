using Domain.Common;
namespace Domain.Entities
{
    public class Module : AuditableAndSoftDeletableEntity
    {
        public string Nom { get; private set; } = null!;
        public string Contenu { get; private set; } = null!;
        public string Sujet { get; private set; } = null!;


        public void SanitizeForSaving()
        {
            Nom = Nom.Trim();  
            Contenu = Contenu.Trim();
            Sujet = Sujet.Trim();
        }

    }
}