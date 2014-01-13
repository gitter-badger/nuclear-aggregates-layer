using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.DTO.ForRelease
{
    public sealed class AdvertisingMaterialInfo
    {
        public long? Id { get; set; }
        public bool? IsSelectedToWhiteList { get; set; }
        public IEnumerable<long> StableAddrIds { get; set; }
        public IEnumerable<long> StableRubrIds { get; set; }
        public IEnumerable<long> ThemeIds { get; set; }
        public IEnumerable<CategoryInAddressElementInfo> RubrInAddrIds { get; set; }
        public IEnumerable<AdvertisingElementInfo> Elements { get; set; }
    }
}