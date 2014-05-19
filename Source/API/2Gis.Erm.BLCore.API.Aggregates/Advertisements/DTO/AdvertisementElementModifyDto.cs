using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO
{
    public sealed class AdvertisementElementModifyDto
    {
        public long AdvertisementId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDummy { get; set; }
        public AdvertisementElement Element { get; set; }
        public AdvertisementElementTemplate ElementTemplate { get; set; }
        public AdvertisementElementStatus PreviousStatusElementStatus { get; set; }
        public IEnumerable<AdvertisementElement> ClonedDummies { get; set; }
    }
}