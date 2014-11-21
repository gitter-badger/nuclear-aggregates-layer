using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO
{
    public sealed class AdvertisementBagItem
    {
        public long Id { get; set; }
        public string Name { get; set; }

        // text
        public string Text { get; set; }

        // file
        public long? FileId { get; set; }
        public string FileName { get; set; }

        // date
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool FormattedText { get; set; }
        public AdvertisementElementRestrictionType RestrictionType { get; set; }
        public string RestrictionName { get; set; }

        public bool IsValid { get; set; }

        public AdvertisementElementStatusValue Status { get; set; }

        public bool NeedsValidation { get; set; }
        public bool UserCanValidateAdvertisement { get; set; }
    }
}