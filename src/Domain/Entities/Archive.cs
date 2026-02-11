using Domain.Common;

namespace Domain.Entities
{
    public class Archive : AuditableAndSoftDeletableEntity
    {

        public Guid? ProgressionId { get; private set; }
        public Progression? Progression { get; private set; }

        public Guid? RdvId { get; private set; }
        public Rdv? Rdv { get; private set; }

        public Guid? EquipeId { get; private set; }
        public Equipe? Equipe { get; private set; }

        private Archive() { }

        public Archive( Guid? progressionId, Guid? rdvId, Guid? equipeId)
        {
            ProgressionId = progressionId;
            RdvId = rdvId;
            EquipeId = equipeId;
        }
    }
}
