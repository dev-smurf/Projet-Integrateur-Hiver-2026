using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services.Equipe.Dto
{
    public class MemberEquipeDto
    {
        public string Id { get; set; } = null!;
        public string MemberId { get; set; } = null!;
        public string EquipeId { get; set; } = null!;
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }  
    }
}
