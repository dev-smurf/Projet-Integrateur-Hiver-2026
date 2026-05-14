using Domain.Common;

namespace Domain.Entities;

public class EquipeNote : AuditableAndSoftDeletableEntity
{
    public Guid EquipeId { get; private set; }
    public Equipe Equipe { get; private set; } = null!;

    public Guid CreatedByAdminId { get; private set; }
    public Administrator CreatedByAdmin { get; private set; } = null!;

    public string Content { get; private set; } = null!;
    
    // Si True, seul l'admin peut voir. Si False, les membres de l'équipe peuvent aussi la voir.
    public bool IsPrivate { get; private set; } = true;

    private EquipeNote() { }

    public EquipeNote(Guid equipeId, Guid createdByAdminId, string content, bool isPrivate = true)
    {
        EquipeId = equipeId;
        CreatedByAdminId = createdByAdminId;
        Content = content;
        IsPrivate = isPrivate;
    }

    public void UpdateContent(string content)
    {
        Content = content;
    }

    public void SetPrivacy(bool isPrivate)
    {
        IsPrivate = isPrivate;
    }
}
