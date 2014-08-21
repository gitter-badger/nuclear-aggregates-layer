using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class AdvertisementElementStatusDomainEntityDto : IAdvertisementElementRestrictions,
                                                                     ITextAdvertisementElementDomainEntityDto, 
                                                                     IPeriodAdvertisementElementDomainEntityDto,
                                                                     IFasCommentAdvertisementElementDomainEntityDto,
                                                                     IFileAdvertisementElementDomainEntityDto,
                                                                     ILinkAdvertisementElementDomainEntityDto,
                                                                     IAdvertisementElementTimestampDomainEntityDto
    {
        [DataMember]
        public ReasonDto[] Reasons { get; set; }

        [DataMember]
        public string PlainText { get; set; }
        [DataMember]
        public string FormattedText { get; set; }

        [DataMember]
        public AdvertisementElementRestrictionType TemplateRestrictionType { get; set; }
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
        public DateTime? BeginDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public FasComment? FasCommentType { get; set; }

        [DataMember]
        public long? FileId { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public long FileContentLength { get; set; }
        [DataMember]
        public string FileContentType { get; set; }

        [DataMember]
        byte[] IAdvertisementElementTimestampDomainEntityDto.Timestamp { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class ReasonDto
    {
        public long Id { get; set; }
        public string Comment { get; set; }
    }
}