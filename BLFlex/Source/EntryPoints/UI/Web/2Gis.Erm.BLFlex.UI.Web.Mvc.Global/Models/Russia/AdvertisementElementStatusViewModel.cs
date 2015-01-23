using System;
using System.Web.Helpers;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels;
using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia
{
    public sealed class AdvertisementElementStatusViewModel : EntityViewModelBase<AdvertisementElementStatus>, IRussiaAdapted
    {
        public AdvertisementElementStatusValue Status { get; set; }
        public string Reasons { get; set; }
        public override byte[] Timestamp { get; set; }

        public AdvertisementElementRestrictionActualType ActualType { get; set; }
        public PeriodViewModel Period { get; set; }
        public FileViewModel File { get; set; }
        public ImageViewModel Image { get; set; }
        public LinkViewModel Link { get; set; }
        public PlainTextViewModel PlainText { get; set; }
        public FasCommentViewModel FasComment { get; set; }
        public FormattedTextViewModel FormattedText { get; set; }
        public byte[] AdvertisementElementTimestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdvertisementElementStatusDomainEntityDto)domainEntityDto;

            Id = dto.Id;
            Status = (AdvertisementElementStatusValue)dto.Status;
            Reasons = Json.Encode(dto.Reasons);
            Timestamp = dto.Timestamp;
            AdvertisementElementTimestamp = ((IAdvertisementElementTimestampDomainEntityDto)dto).Timestamp;

            ActualType = GetActualType(dto.TemplateRestrictionType,
                                       dto.TemplateAdvertisementLink,
                                       dto.TemplateFormattedText);
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
                    FasComment.FasCommentDisplayTextItemsJson = AdvertisementElementViewModelCustomizationService.GetDisplayTextItemsJson();
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
            var dto = new AdvertisementElementStatusDomainEntityDto
                {
                    Id = Id,
                    Reasons = Json.Decode<ReasonDto[]>(Reasons),
                    Timestamp = Timestamp,
                    Status = (int)Status,
                };

            ((IAdvertisementElementTimestampDomainEntityDto)dto).Timestamp = AdvertisementElementTimestamp;

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
