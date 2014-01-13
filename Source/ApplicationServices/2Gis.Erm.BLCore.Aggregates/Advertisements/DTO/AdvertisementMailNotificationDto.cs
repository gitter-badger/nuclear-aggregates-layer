using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.DTO
{
    public sealed class AdvertisementMailNotificationDto
    {
        public LinkDto Firm { get; set; }
        public long FirmOwnerCode { get; set; }

        public LinkDto Advertisement { get; set; }
        public string AdvertisementTemplateName { get; set; }
        public string AdvertisementElementTemplateName { get; set; }

        public IEnumerable<LinkDto> Orders { get; set; }
    }

    // FIXME {v.lapeev, 28.10.2013}: Заменить использование этого класса на EntityReference, этот класс удалить
    public sealed class LinkDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}