using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO
{
    public sealed class AdvertisementMailNotificationDto
    {
        public EntityReference FirmRef { get; set; }

        public EntityReference AdvertisementRef { get; set; }
        public string AdvertisementTemplateName { get; set; }
        public string AdvertisementElementTemplateName { get; set; }

        public IEnumerable<OrderInfo> OrderRefs { get; set; }

        public class OrderInfo
        {
            public long Id { get; set; }
            public string Number { get; set; }
            public long OwnerCode { get; set; }
        }
    }
}