using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class ActionsHistoryDetail :
        IEntity,
        IEntityKey,
        IStateTrackingEntity
    {
        public long Id { get; set; }
        public long ActionsHistoryId { get; set; }
        public string PropertyName { get; set; }
        public string OriginalValue { get; set; }
        public string ModifiedValue { get; set; }
        public byte[] Timestamp { get; set; }

        public ActionsHistory ActionsHistory { get; set; }

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