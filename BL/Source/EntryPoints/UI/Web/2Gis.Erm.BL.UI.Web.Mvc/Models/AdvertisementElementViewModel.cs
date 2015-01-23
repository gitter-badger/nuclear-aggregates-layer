using System;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AdvertisementElementViewModel : EntityViewModelBase<AdvertisementElement>
    {
        [DisplayNameLocalized("AdvertisementElementTemplateName")]
        [RequiredLocalized]
        public LookupField AdvertisementElementTemplate { get; set; }

        public AdvertisementElementRestrictionActualType ActualType { get; set; }
        public PeriodViewModel Period { get; set; }
        public FileViewModel File { get; set; }
        public ImageViewModel Image { get; set; }
        public LinkViewModel Link { get; set; }
        public PlainTextViewModel PlainText { get; set; }
        public FasCommentViewModel FasComment { get; set; }
        public FormattedTextViewModel FormattedText { get; set; }

        public override byte[] Timestamp { get; set; }

        public AdvertisementElementStatusValue Status { get; set; }

        public bool NeedsValidation { get; set; }

        public bool CanUserChangeStatus { get; set; }

        public bool DisableEdit { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdvertisementElementDomainEntityDto)domainEntityDto;

            Id = dto.Id;
            Status = dto.Status;
            AdvertisementElementTemplate = LookupField.FromReference(dto.AdvertisementElementTemplateRef);
            Timestamp = dto.Timestamp;
            NeedsValidation = dto.NeedsValidation;
            CanUserChangeStatus = dto.CanUserChangeStatus;
            DisableEdit = dto.DisableEdit;

            ActualType = GetActualType(dto.TemplateRestrictionType, dto.TemplateAdvertisementLink, dto.TemplateFormattedText);
            switch (ActualType)
            {
                case AdvertisementElementRestrictionActualType.File:
                    File = new FileViewModel();
                    File.LoadDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.Date:
                    Period = new PeriodViewModel();
                    Period.LoadDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.FasComment:
                    FasComment = new FasCommentViewModel();
                    FasComment.LoadDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.FormattedText:
                    FormattedText = new FormattedTextViewModel();
                    FormattedText.LoadDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.Image:
                    Image = new ImageViewModel();
                    Image.LoadDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.Link:
                    Link = new LinkViewModel();
                    Link.LoadDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.PlainText:
                    PlainText = new PlainTextViewModel();
                    PlainText.LoadDomainEntityDto(dto);
                    break;
            }
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (AdvertisementElementTemplate.Key == null)
            {
                throw new ArgumentNullException();
            }

            var dto = new AdvertisementElementDomainEntityDto
                {
                    Id = Id,
                    AdvertisementElementTemplateRef = AdvertisementElementTemplate.ToReference(),

                    // TODO: Выпилить OwnerCode
                    OwnerRef = new EntityReference(Owner != null && Owner.Key.HasValue ? Owner.Key.Value : 0),
                    Timestamp = Timestamp
                };

            switch (ActualType)
            {
                case AdvertisementElementRestrictionActualType.File:
                    File.TranferToDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.Date:
                    Period.TransferToDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.FasComment:
                    FasComment.TransferToDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.FormattedText:
                    FormattedText.TransferToDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.Image:
                    Image.TransferToDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.Link:
                    Link.TransferToDomainEntityDto(dto);
                    break;
                case AdvertisementElementRestrictionActualType.PlainText:
                    PlainText.TransferToDomainEntityDto(dto);
                    break;
            }

            return dto;
        }

        private static AdvertisementElementRestrictionActualType GetActualType(AdvertisementElementRestrictionType templateRestrictionType,
                                                                               bool isAdvertisementLink,
                                                                               bool isFormattedText)
        {
            switch (templateRestrictionType)
            {
                case AdvertisementElementRestrictionType.Article:
                    return AdvertisementElementRestrictionActualType.File;
                case AdvertisementElementRestrictionType.Date:
                    return AdvertisementElementRestrictionActualType.Date;
                case AdvertisementElementRestrictionType.FasComment:
                    return AdvertisementElementRestrictionActualType.FasComment;
                case AdvertisementElementRestrictionType.Image:
                    return AdvertisementElementRestrictionActualType.Image;
                case AdvertisementElementRestrictionType.Text:
                    if (isAdvertisementLink)
                    {
                        return AdvertisementElementRestrictionActualType.Link;
                    }

                    if (isFormattedText)
                    {
                        return AdvertisementElementRestrictionActualType.FormattedText;
                    }

                    return AdvertisementElementRestrictionActualType.PlainText;
                default:
                    throw new ArgumentException("templateRestrictionType");
            }
        }
    }
}
