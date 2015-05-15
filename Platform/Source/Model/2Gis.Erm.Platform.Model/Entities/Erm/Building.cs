using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Building : IEntity,
                                   IDeletableEntity,
                                   IImported
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
    }
}