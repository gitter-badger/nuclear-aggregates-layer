using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class FirmAddressService :
        IEntity,
        IEntityKey,
        IStateTrackingEntity
    {
        public long Id { get; set; }
        public long FirmAddressId { get; set; }
        public long ServiceId { get; set; }
        public bool DisplayService { get; set; }
        public byte[] Timestamp { get; set; }

        public FirmAddress FirmAddress { get; set; }
        public AdditionalFirmService AdditionalFirmService { get; set; }

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