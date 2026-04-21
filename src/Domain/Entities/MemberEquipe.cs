using System;

namespace Domain.Entities;

public class MemberEquipe
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public Guid EquipeId { get; set; }

    // Populated by repositories when needed by the Web layer.
    public Member? Member { get; set; }

    public MemberEquipe() { }

    public MemberEquipe(Guid memberId, Guid equipeId)
    {
        Id = Guid.NewGuid();
        MemberId = memberId;
        EquipeId = equipeId;
    }
}

