using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Domain.Entities
{
    public class Module : AuditableAndSoftDeletableEntity
    {
        public string? NameFr { get; set; }
        public string? NameEn { get; set; }
        public string? ContenueFr { get; set; }
        public string? ContenueEn { get; set; }
        [NotMapped]
        public IFormFile? CardImage { get; set; }
        public string? CardImageUrl {get;set;}
        public string? SujetFr { get; set; }
        public string? SujetEn { get; set; }

        public void SanitizeForSaving()
        {
            if (NameFr != null) NameFr = NameFr.Trim();
            if (NameEn != null) NameEn = NameEn.Trim();
            if (ContenueFr != null) ContenueFr = ContenueFr.Trim();
            if (ContenueEn != null) ContenueEn = ContenueEn.Trim();
            if (SujetFr != null) SujetFr = SujetFr.Trim();
            if (SujetEn != null) SujetEn = SujetEn.Trim();
        }
    }
}
