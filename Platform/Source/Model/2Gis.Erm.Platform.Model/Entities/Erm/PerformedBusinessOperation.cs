using System;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class PerformedBusinessOperation :
        IEntity,
        IEntityKey
    {
        public long Id { get; set; }
        public int Operation { get; set; }
        public int Descriptor { get; set; }
        public string Context { get; set; }
        public DateTime Date { get; set; }
        public long? Parent { get; set; }
        public Guid UseCaseId { get; set; }
        public string OperationEntities { get; set; }

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