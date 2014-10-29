using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    // FIXME {a.rechkalov, 17.09.2014}: Этот класс и функционал, связанный с ним не нужны больше
    public sealed class OrdersRegionalAdvertisingSharing :
        IEntity,
        IEntityKey,
        IStateTrackingEntity
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long RegionalAdvertisingSharingId { get; set; }
        public byte[] Timestamp { get; set; }

        public Order Order { get; set; }
        public RegionalAdvertisingSharing RegionalAdvertisingSharing { get; set; }

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