using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AdvertisementElementViewModel : FileViewModel<AdvertisementElement>
    {
        [DisplayNameLocalized("AdvertisementElementTemplateName")]
        [RequiredLocalized]
        public LookupField AdvertisementElementTemplate { get; set; }

        public string TemplateFileExtensionRestriction { get; set; }
        public string TemplateImageDimensionRestriction { get; set; }

        [Dependency(DependencyType.Required, "BeginDate", "this.value=='FasComment'")]
        public AdvertisementElementRestrictionType TemplateRestrictionType { get; set; }

        public int? TemplateTextLengthRestriction { get; set; }
        public byte? TemplateMaxSymbolsInWord { get; set; }

        public int? TemplateTextLineBreaksRestriction { get; set; }
        public bool TemplateFormattedText { get; set; }
        public bool TemplateAdvertisementLink { get; set; }

        [DisplayNameLocalized("Text")]
        public string FormattedText { get; set; }

        [DisplayNameLocalized("Text")]
        public string PlainText { get; set; }

        [Dependency(DependencyType.Transfer, "PlainText", "var t = Ext.decode(Ext.getDom('FasCommentDisplayTextItems').value); initially?undefined:t[this.value];")]
        [Dependency(DependencyType.ReadOnly, "PlainText", "this.value!='NewFasComment'")]
        public FasComment? FasComment { get; set; }

        [PresentationLayerProperty]
        public string FasCommentDisplayTextItemsJson { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public long? FileId { get; set; }

        public override byte[] Timestamp { get; set; }

        // Пустая строка для this.value означает, что будет выбрано значение enum, соответствующее 0, т.е. AdvertisementElementError.Absent
        [Dependency(DependencyType.Disable, "Error", "Ext.getDom('CanUserChangeStatus').value.toLowerCase() === 'true' ? this.value != 'Invalid' && this.value != 'InvalidAfterEdit' : true")]
        [Dependency(DependencyType.Required, "Error", "this.value == 'Invalid'")]
        [Dependency(DependencyType.Transfer, "Error", @"this.value=='Valid' ? '' : Ext.getDom('Error').value")]

        public AdvertisementElementStatus Status { get; set; }

        [DisplayNameLocalized("AdvertisementElementErrorDescription")]
        public AdvertisementElementError Error { get; set; }

        public bool NeedsValidation { get; set; }

        public bool CanUserChangeStatus { get; set; }

        public AdvertisementElementStatus[] AvailableStates { get; set; }

        public bool UserDoesntHaveRightsToEditFirm { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var advertisementElementDto = (AdvertisementElementDomainEntityDto)domainEntityDto;

            Id = advertisementElementDto.Id;
            BeginDate = advertisementElementDto.BeginDate;
            EndDate = advertisementElementDto.EndDate;
            AdvertisementElementTemplate = LookupField.FromReference(advertisementElementDto.AdvertisementElementTemplateRef);
            TemplateFileExtensionRestriction = advertisementElementDto.TemplateFileExtensionRestriction;
            TemplateImageDimensionRestriction = advertisementElementDto.TemplateImageDimensionRestriction;
            TemplateFormattedText = advertisementElementDto.TemplateFormattedText;
            TemplateAdvertisementLink = advertisementElementDto.TemplateAdvertisementLink;
            TemplateTextLengthRestriction = advertisementElementDto.TemplateTextLengthRestriction;
            TemplateMaxSymbolsInWord = advertisementElementDto.TemplateMaxSymbolsInWord;
            TemplateTextLineBreaksRestriction = advertisementElementDto.TemplateTextLineBreaksRestriction;
            TemplateRestrictionType = advertisementElementDto.TemplateRestrictionType;
            FasComment = advertisementElementDto.FasCommentType;
            PlainText = advertisementElementDto.PlainText;
            FormattedText = advertisementElementDto.FormattedText;
            FileId = advertisementElementDto.FileId;
            FileName = advertisementElementDto.FileName;
            FileContentLength = advertisementElementDto.FileContentLength;
            FileContentType = advertisementElementDto.FileContentType;
            Timestamp = advertisementElementDto.Timestamp;
            NeedsValidation = advertisementElementDto.NeedsValidation;
            CanUserChangeStatus = advertisementElementDto.CanUserChangeStatus;
            UserDoesntHaveRightsToEditFirm = advertisementElementDto.UserDoesntHaveRightsToEditFirm;
            Status = advertisementElementDto.Status;
            Error = advertisementElementDto.Error;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (AdvertisementElementTemplate.Key == null)
            {
                throw new ArgumentNullException();
            }

            // Ограничение UI: DropDownList возвращает null в качестве значения nullable-enum, соответствующего 0
            if (FasComment == null && TemplateRestrictionType == AdvertisementElementRestrictionType.FasComment)
            {
                FasComment = 0;
            }

            return new AdvertisementElementDomainEntityDto
                {
                    Id = Id,
                    BeginDate = BeginDate,
                    EndDate = EndDate,
                    AdvertisementElementTemplateRef = AdvertisementElementTemplate.ToReference(),
                    TemplateFileExtensionRestriction = TemplateFileExtensionRestriction,
                    TemplateImageDimensionRestriction = TemplateImageDimensionRestriction,
                    TemplateFormattedText = TemplateFormattedText,
                    TemplateAdvertisementLink = TemplateAdvertisementLink,
                    TemplateTextLengthRestriction = TemplateTextLengthRestriction,
                    TemplateMaxSymbolsInWord = TemplateMaxSymbolsInWord,
                    TemplateTextLineBreaksRestriction = TemplateTextLineBreaksRestriction,
                    TemplateRestrictionType = TemplateRestrictionType,
                    FasCommentType = FasComment,
                    PlainText = PlainText,
                    FormattedText = Unescape(FormattedText),
                    FileId = FileId,
                    FileName = FileName,
                    FileContentLength = FileContentLength,
                    FileContentType = FileContentType,
                    Error = Error,
                    Status = Status,

                    // TODO: Выпилить OwnerCode
                    OwnerRef = new EntityReference(Owner != null && Owner.Key.HasValue ? Owner.Key.Value : 0),
                    Timestamp = Timestamp
                };
        }

        private static string Unescape(string formattedText)
        {
            // делаем unescape на форматированный текст, т.к. до этого в js на него делаем escape
            // а escape в js мы сделали чтобы не передавать html в теле запроса, asp.net mvc ругается что это небезопасно
            string unescapedText = formattedText;
            if (!string.IsNullOrEmpty(formattedText))
            {
                // decode formatted text
                unescapedText = Uri.UnescapeDataString(unescapedText).Replace("\u001d", string.Empty);
                unescapedText = Uri.UnescapeDataString(unescapedText).Replace("&nbsp;", " ");
            }

            return unescapedText;
        }
    }
}
