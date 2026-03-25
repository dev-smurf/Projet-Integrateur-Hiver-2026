using Domain.Common;

namespace Domain.Entities
{
    public class Module : AuditableAndSoftDeletableEntity
    {
        public string NameFr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string? ContenueFr { get; set; }
        public string? ContenueEn { get; set; }
        public string? SujetFr { get; set; }
        public string? SujetEn { get; set; }
        public string? CardImageUrl { get; set; }
        public ICollection<MemberModule> MemberModules { get; private set; } = new List<MemberModule>();
    }
}
