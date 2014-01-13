using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.ServiceBus.Export
{
    public sealed class SerializeAdvertisementHandler : SerializeObjectsHandler<Advertisement>
    {
        private readonly IFileService _fileService;

        public SerializeAdvertisementHandler(IExportRepository<Advertisement> exportRepository,
                                             IFileService fileService,
                                             ICommonLog logger)
            : base(exportRepository, logger)
        {
            _fileService = fileService;
        }

        protected override string GetError(IExportableEntityDto entiryDto)
        {
            var advertisementDto = (AdvertisementDto)entiryDto;

            foreach (var element in advertisementDto.Elements)
            {
                switch (element.RestrictionType)
                {
                    case AdvertisementElementRestrictionType.Text:
                    case AdvertisementElementRestrictionType.FasComment:
                        if (element.IsRequired && string.IsNullOrEmpty(element.Text))
                        {
                            // TODO {y.baranihin, 02.07.2013}: Все же не стоит использовать перегрузку string.Format с указанием CultureInfo.CurrentCulture
                            //                                  -> 1. CultureInfo.CurrentCulture в разных точках входа в приложение потенциально может разным
                            //                                  -> 2. в string.Format этот IFormatProvider нужен для локализации параметров, не самой строки, т.е. в русской строке могут оказаться параметры с en-US форматированием
                            //                                  -> 3. если требуется локализация, то нужно все выносить в ресурсы и использовать IUserContext.Profile.UserLocaleInfo.UserCultureInfo
                            return string.Format(
                                CultureInfo.CurrentCulture,
                                "Рекламный материал с id=[{0}] имеет незаполненный обязательный элемент c id=[{1}]",
                                advertisementDto.Id,
                                element.Id);
                        }

                        break;
                    case AdvertisementElementRestrictionType.Date:
                        if (element.IsRequired && (element.BeginDate == null || element.EndDate == null))
                        {
                            return string.Format(
                                CultureInfo.CurrentCulture,
                                "Рекламный материал с id=[{0}] имеет незаполненный обязательный элемент c id=[{1}]",
                                advertisementDto.Id,
                                element.Id);
                        }

                        break;
                    case AdvertisementElementRestrictionType.Article:
                    case AdvertisementElementRestrictionType.Image:
                        if (element.IsRequired && element.FileId == null)
                        {
                            return string.Format(
                                CultureInfo.CurrentCulture,
                                "Рекламный материал с id=[{0}] имеет незаполненный обязательный элемент c id=[{1}]",
                                advertisementDto.Id,
                                element.Id);
                        }

                        break;
                }
            }

            return null;
        }

        protected override XElement SerializeDtoToXElement(IExportableEntityDto entiryDto)
        {
            var advertisementDto = (AdvertisementDto)entiryDto;

            var advMaterialElement = new XElement("AdvMaterial",
                new XAttribute("Code", advertisementDto.Id),
                new XAttribute("IsWhiteListed", advertisementDto.IsSelectedToWhiteList),
                advertisementDto.FirmCode.HasValue ? new XAttribute("FirmCode", advertisementDto.FirmCode.Value) : null,
                CreateValidationStatusAttribute(advertisementDto.Elements),
                CreateValidationErrorAttribute(advertisementDto.Elements));

            if (advertisementDto.IsDeleted)
            {
                // у нас нету отдельного атрибута IsHidden, сейчас его поведение аналогично IsDeleted
                advMaterialElement.Add(new XAttribute("IsHidden", true));
                advMaterialElement.Add(new XAttribute("IsDeleted", true));
            }

            var elementsElement = GetElementsElement(advertisementDto);
            advMaterialElement.Add(elementsElement);

            return advMaterialElement;
        }

        protected override ISelectSpecification<Advertisement, IExportableEntityDto> CreateDtoExpression()
        {
            return new SelectSpecification<Advertisement, IExportableEntityDto>(x => new AdvertisementDto
            {
                Id = x.Id,

                IsDeleted = x.IsDeleted,
                IsSelectedToWhiteList = x.IsSelectedToWhiteList,
                FirmCode = x.Firm.Id,

                // elements
                Elements = x.AdvertisementElements.Where(z => !z.IsDeleted).Select(z => new ElementDto
                {
                    Id = z.Id,
                    RestrictionType = (AdvertisementElementRestrictionType)z.AdvertisementElementTemplate.RestrictionType,
                    ExportCode = z.AdsTemplatesAdsElementTemplate.ExportCode,
                    IsRequired = z.AdvertisementElementTemplate.IsRequired,

                    NeedsValidation = z.AdvertisementElementTemplate.NeedsValidation,
                    ValidationStatus = (AdvertisementElementStatus)z.Status,
                    ReasonForReject = (AdvertisementElementError)z.Error,

                    Text = z.Text,
                    FileId = z.FileId,
                    BeginDate = z.BeginDate,
                    EndDate = z.EndDate,
                }),
                });
        }

        private XElement GetElementsElement(AdvertisementDto advertisementDto)
        {
            var elementsElement = new XElement("Elements");

            foreach (var element in advertisementDto.Elements)
            {
                switch (element.RestrictionType)
                {
                    case AdvertisementElementRestrictionType.Text:
                    case AdvertisementElementRestrictionType.FasComment:
                        {
                            var text = element.Text ?? string.Empty;

                            elementsElement.Add(new XElement("Text",
                                                             new XAttribute("TemplateCode", element.ExportCode),
                                                             new XAttribute("Value", text)));
                        }

                        continue;

                    case AdvertisementElementRestrictionType.Date:
                        {
                            if (element.BeginDate == null)
                            {
                                element.BeginDate = DateTime.MinValue;
                            }

                            if (element.EndDate == null)
                            {
                                element.EndDate = DateTime.MaxValue;
                            }

                            elementsElement.Add(new XElement("TimePeriod",
                                                             new XAttribute("TemplateCode", element.ExportCode),
                                                             new XAttribute("From", element.BeginDate.Value),
                                                             new XAttribute("To", element.EndDate.Value)));
                        }

                        continue;

                    case AdvertisementElementRestrictionType.Article:
                    case AdvertisementElementRestrictionType.Image:
                        {
                            string content;

                            if (element.FileId != null)
                            {
                                var memoryStream = new MemoryStream();
                                var fileContent = _fileService.GetFileContent(element.FileId.Value);
                                fileContent.CopyTo(memoryStream);
                                content = Convert.ToBase64String(memoryStream.ToArray());
                            }
                            else
                            {
                                content = string.Empty;
                            }

                            elementsElement.Add(new XElement("Blob",
                                                             new XAttribute("TemplateCode", element.ExportCode),
                                                             new XAttribute("Value", content)));
                        }

                        continue;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return elementsElement;
        }

        private XAttribute CreateValidationStatusAttribute(IEnumerable<ElementDto> advElements)
        {
            string resultStatus;
            if (advElements.Any(x => x.NeedsValidation && x.ValidationStatus == AdvertisementElementStatus.Invalid))
            {
                resultStatus = "Rejected";
            }
            else if (advElements.Any(x => x.NeedsValidation && x.ValidationStatus == AdvertisementElementStatus.NotValidated))
            {
                resultStatus = "OnApproval";
            }
            else
            {
                // ЭРМ, не требующие выверки, так же считаются валидными
                resultStatus = "Approved";
            }

            return new XAttribute("Status", resultStatus);
        }

        private XAttribute CreateValidationErrorAttribute(IEnumerable<ElementDto> advElements)
        {
            var firstRejectedAdvElement = advElements.FirstOrDefault(dto => dto.ValidationStatus == AdvertisementElementStatus.Invalid);
            return firstRejectedAdvElement == null
                       ? null
                       : new XAttribute("ReasonForRejectCode", (int)firstRejectedAdvElement.ReasonForReject);
        }

        #region nested types

        private sealed class AdvertisementDto : IExportableEntityDto
        {
            public long Id { get; set; }

            public bool IsDeleted { get; set; }
            public bool IsSelectedToWhiteList { get; set; }
            public long? FirmCode { get; set; }

            public IEnumerable<ElementDto> Elements { get; set; }
        }

        private sealed class ElementDto
        {
            public long Id { get; set; }
            public AdvertisementElementRestrictionType RestrictionType { get; set; }
            public int ExportCode { get; set; }
            public bool IsRequired { get; set; }

            public AdvertisementElementStatus ValidationStatus { get; set; }
            public AdvertisementElementError ReasonForReject { get; set; }
            public bool NeedsValidation { get; set; }

            public string Text { get; set; }
            public long? FileId { get; set; }
            public DateTime? BeginDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        #endregion
    }
}