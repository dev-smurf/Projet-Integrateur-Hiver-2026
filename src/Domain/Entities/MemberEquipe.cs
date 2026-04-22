using Domain.Common;

namespace Domain.Entities;

public class MemberEquipe : AuditableAndSoftDeletableEntity
{
    public Guid MemberId { get; private set; }
    public Member Member { get; private set; } = null!;

    public Guid EquipeId { get; private set; }
    public Equipe Equipe { get; private set; } = null!;

    private MemberEquipe() { }

    public MemberEquipe(Guid memberId, Guid equipeId)
    {
        MemberId = memberId;
        EquipeId = equipeId;
    }
}