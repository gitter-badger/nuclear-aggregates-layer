using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class AdvertisementElementDomainEntityDto : IDomainEntityDto<AdvertisementElement>,
                                                       IAdvertisementElementRestrictions,
                                                       ITextAdvertisementElementDomainEntityDto,
                                                       IPeriodAdvertisementElementDomainEntityDto,
                                                       IFasCommentAdvertisementElementDomainEntityDto,
                                                       IFileAdvertisementElementDomainEntityDto,
                                                       ILinkAdvertisementElementDomainEntityDto,
                                                       IAdvertisementElementTimestampDomainEntityDto
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference AdvertisementRef { get; set; }

        [DataMember]
        public EntityReference AdvertisementElementTemplateRef { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public long? FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public DateTime? BeginDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public FasComment? FasCommentType { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public EntityReference OwnerRef { get; set; }

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
        public EntityReference AdsTemplatesAdsElementTemplatesRef { get; set; }

        [DataMember]
        public long? DgppId { get; set; }

        [DataMember]
        public string TemplateFileExtensionRestriction { get; set; }

        [DataMember]
        public string TemplateImageDimensionRestriction { get; set; }

        [DataMember]
        public bool TemplateFormattedText { get; set; }

        [DataMember]
        public bool TemplateAdvertisementLink { get; set; }

        [DataMember]
        public int? TemplateTextLengthRestriction { get; set; }

        [DataMember]
        public byte? TemplateMaxSymbolsInWord { get; set; }

        [DataMember]
        public int? TemplateTextLineBreaksRestriction { get; set; }

        [DataMember]
        public AdvertisementElementRestrictionType TemplateRestrictionType { get; set; }

        [DataMember]
        public string PlainText { get; set; }

        [DataMember]
        public string FormattedText { get; set; }

        [DataMember]
        public long FileContentLength { get; set; }

        [DataMember]
        public string FileContentType { get; set; }

        [DataMember]
        public bool NeedsValidation { get; set; }

        [DataMember]
        public bool CanUserChangeStatus { get; set; }

        [DataMember]
        public bool SetReadonly { get; set; }

        [DataMember]
        public AdvertisementElementStatusValue Status { get; set; }
    }
}