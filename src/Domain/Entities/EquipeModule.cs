using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

public class EquipeModule
{
    public Guid EquipeId { get; private set; }
    public Guid ModuleId { get; private set; }

    public DateTime AssignedAt { get; private set; }

    public virtual Equipe Equipe { get; private set; } = null!;
    public virtual Module Module { get; private set; } = null!;

    private EquipeModule() { }

    public EquipeModule(Guid equipeId, Guid moduleId)
    {
        EquipeId = equipeId;
        ModuleId = moduleId;
        AssignedAt = DateTime.UtcNow;
    }
}