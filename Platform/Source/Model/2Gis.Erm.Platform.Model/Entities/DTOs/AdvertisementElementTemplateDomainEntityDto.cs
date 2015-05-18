using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class AdvertisementElementTemplateDomainEntityDto : IDomainEntityDto<AdvertisementElementTemplate>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int? TextLengthRestriction { get; set; }

        [DataMember]
        public byte? MaxSymbolsInWord { get; set; }

        [DataMember]
        public int? TextLineBreaksCountRestriction { get; set; }

        [DataMember]
        public bool FormattedText { get; set; }

        [DataMember]
        public int? FileSizeRestriction { get; set; }

        [DataMember]
        public string FileExtensionRestriction { get; set; }

        [DataMember]
        public int? FileNameLengthRestriction { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }

        [DataMember]
        public AdvertisementElementRestrictionType RestrictionType { get; set; }

        [DataMember]
        public bool IsRequired { get; set; }

        [DataMember]
        public string ImageDimensionRestriction { get; set; }

        [DataMember]
        public bool IsAdvertisementLink { get; set; }

        [DataMember]
        public bool NeedsValidation { get; set; }

        [DataMember]
        public EntityReference DummyAdvertisementElementRef { get; set; }

        // supported file types
        public bool IsPngSupported { get; set; }
        public bool IsGifSupported { get; set; }
        public bool IsBmpSupported { get; set; }

        // supported article types
        public bool IsChmSupported { get; set; }
    }
}