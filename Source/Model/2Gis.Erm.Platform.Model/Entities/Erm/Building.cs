using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed partial class Building :
        IEntity,
        IDeletableEntity
    {
        public Building()
        {
            FirmAddresses = new HashSet<FirmAddress>();
        }

        public long Code { get; set; }
        public long TerritoryId { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<FirmAddress> FirmAddresses { get; set; }
        public Territory Territory { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}