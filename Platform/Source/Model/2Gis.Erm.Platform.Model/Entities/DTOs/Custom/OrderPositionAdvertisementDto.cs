using System.Collections.Generic;

using DoubleGis.Erm.Core.Utils;

namespace DoubleGis.Erm.Core.Dto.DomainEntity.Custom
{
    public sealed class OrderPositionAdvertisementDto
    {
        public List<OrderPositionAdvertisementDto> Children { get; set; }
        public LinkingObjectType Type { get; set; }
        public string Name { get; set; }
        public bool CanBeChecked { get; set; }
        public bool CanBeLinked { get; set; }
        public bool IsChecked { get; set; }
        public EntityReference AdvertisementLink { get; set; }
    }
}
