using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO.ForRelease
{
    public sealed class OrderPositionInfo
    {
        public long Id { get; set; }
        public long? PlatformId { get; set; }
        public long? ProductType { get; set; }
        public long? ProductCategory { get; set; }
        public IEnumerable<AdvertisingMaterialInfo> AdvertisingMaterials { get; set; }
    }
}