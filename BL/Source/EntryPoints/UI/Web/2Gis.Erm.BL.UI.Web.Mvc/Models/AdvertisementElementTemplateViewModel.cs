using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AdvertisementElementTemplateViewModel : EditableIdEntityViewModelBase<AdvertisementElementTemplate>
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [Dependency(DependencyType.DisableAndHide, "supportedFilesHeader", "this.value!='Article' && this.value!='Image'")]
        [Dependency(DependencyType.DisableAndHide, "IsPngSupported", "this.value!='Image'")]
        [Dependency(DependencyType.DisableAndHide, "IsGifSupported", "this.value!='Image'")]
        [Dependency(DependencyType.DisableAndHide, "IsBmpSupported", "this.value!='Image'")]
        [Dependency(DependencyType.DisableAndHide, "IsChmSupported", "this.value!='Article'")]
        [Dependency(DependencyType.DisableAndHide, "FileNameLengthRestriction", "this.value!='Article' && this.value!='Image'")]
        [Dependency(DependencyType.DisableAndHide, "FileSizeRestriction", "this.value!='Article' && this.value!='Image'")]
        [Dependency(DependencyType.DisableAndHide, "TextLengthRestriction", "this.value!='Text'&& this.value!='FasComment'")]
        [Dependency(DependencyType.DisableAndHide, "FormattedText", "this.value!='Text'")]
        [Dependency(DependencyType.DisableAndHide, "MaxSymbolsInWord", "this.value!='Text'")]
        [Dependency(DependencyType.DisableAndHide, "IsAdvertisementLink", "this.value!='Text'")]
        [Dependency(DependencyType.DisableAndHide, "ImageDimensionRestriction", "this.value!='Image'")]
        [Dependency(DependencyType.DisableAndHide, "TextLineBreaksCountRestriction", "this.value!='Text'")]
        [RequiredLocalized]
        public AdvertisementElementRestrictionType RestrictionType { get; set; }

        public int? FileSizeRestriction { get; set; }

        public int? FileNameLengthRestriction { get; set; }
        
        public int? TextLineBreaksCountRestriction { get; set; }

        public int? TextLengthRestriction { get; set; }

        public byte? MaxSymbolsInWord { get; set; }

        public bool FormattedText { get; set; }

        [RegularExpression(@"(\d+x\d+[ ]?\|?[ ]?)+$", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "FieldMustMatchImageDimensionRegex")]
        public string ImageDimensionRestriction { get; set; }

        public LookupField DummyAdvertisement { get; set; }

        public bool IsRequired { get; set; }

        public bool NeedsValidation { get; set; }

        public bool IsAdvertisementLink { get; set; }

        public bool IsPngSupported { get; set; }
        public bool IsGifSupported { get; set; }
        public bool IsBmpSupported { get; set; }

        public bool IsChmSupported { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var advertisementElementTemplateDto = (AdvertisementElementTemplateDomainEntityDto)domainEntityDto;

            Name = advertisementElementTemplateDto.Name;
            Id = advertisementElementTemplateDto.Id;
            FileNameLengthRestriction = advertisementElementTemplateDto.FileNameLengthRestriction;
            FileSizeRestriction = advertisementElementTemplateDto.FileSizeRestriction;
            ImageDimensionRestriction = advertisementElementTemplateDto.ImageDimensionRestriction;
            IsRequired = advertisementElementTemplateDto.IsRequired;
            FormattedText = advertisementElementTemplateDto.FormattedText;
            IsAdvertisementLink = advertisementElementTemplateDto.IsAdvertisementLink;
            RestrictionType = advertisementElementTemplateDto.RestrictionType;
            TextLengthRestriction = advertisementElementTemplateDto.TextLengthRestriction;
            MaxSymbolsInWord = advertisementElementTemplateDto.MaxSymbolsInWord;
            TextLineBreaksCountRestriction = advertisementElementTemplateDto.TextLineBreaksCountRestriction;
            NeedsValidation = advertisementElementTemplateDto.NeedsValidation;
            DummyAdvertisement = LookupField.FromReference(advertisementElementTemplateDto.DummyAdvertisementElementRef);
            Timestamp = advertisementElementTemplateDto.Timestamp;
            IsPngSupported = advertisementElementTemplateDto.IsPngSupported;
            IsGifSupported = advertisementElementTemplateDto.IsGifSupported;
            IsBmpSupported = advertisementElementTemplateDto.IsBmpSupported;
            IsChmSupported = advertisementElementTemplateDto.IsChmSupported;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new AdvertisementElementTemplateDomainEntityDto
                {
                    Name = Name,
                    Id = Id,
                    FileNameLengthRestriction = FileNameLengthRestriction,
                    FileSizeRestriction = FileSizeRestriction,
                    ImageDimensionRestriction = ImageDimensionRestriction,
                    IsRequired = IsRequired,
                    FormattedText = FormattedText,
                    IsAdvertisementLink = IsAdvertisementLink,
                    RestrictionType = RestrictionType,
                    TextLengthRestriction = TextLengthRestriction,
                    MaxSymbolsInWord = MaxSymbolsInWord,
                    TextLineBreaksCountRestriction = TextLineBreaksCountRestriction,
                    Timestamp = Timestamp,
                    NeedsValidation = NeedsValidation,
                    IsPngSupported = IsPngSupported,
                    IsGifSupported = IsGifSupported,
                    IsBmpSupported = IsBmpSupported,
                    IsChmSupported = IsChmSupported,
                };
        }
    }
}