using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
// ReSharper restore CheckNamespace
{
    public partial class AdvertisementElementDomainEntityDto
    {
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
    }
}