using System;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class PerformedOperationFinalProcessing :
        IEntity,
        IEntityKey
    {
        public long Id { get; set; }
        public int EntityTypeId { get; set; }
        public long EntityId { get; set; }
        public int AttemptCount { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid MessageFlowId { get; set; }
        public string Context { get; set; }
        public Guid OperationId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var entityKey = obj as IEntityKey;
            if (entityKey != null)
            {
                return Id == entityKey.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}