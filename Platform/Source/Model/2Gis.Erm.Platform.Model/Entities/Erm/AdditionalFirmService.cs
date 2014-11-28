using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class AdditionalFirmService :
        IEntity,
        IEntityKey
    {
        public AdditionalFirmService()
        {
            FirmAddressServices = new HashSet<FirmAddressService>();
        }

        public long Id { get; set; }
        public string ServiceCode { get; set; }
        public bool IsManaged { get; set; }
        public string Description { get; set; }

        public ICollection<FirmAddressService> FirmAddressServices { get; set; }

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