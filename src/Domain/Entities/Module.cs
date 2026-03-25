using Domain.Common;

namespace Domain.Entities
{
    public class Module : AuditableAndSoftDeletableEntity
    {
        public string Name { get; set; } = null!;
        public string? Content { get; set; }
        public string? Subject { get; set; }
        public string? CardImageUrl { get; set; }
        public ICollection<ModuleSection> Sections { get; set; } = new List<ModuleSection>();
        public ICollection<MemberModule> MemberModules { get; private set; } = new List<MemberModule>();
    }
}
