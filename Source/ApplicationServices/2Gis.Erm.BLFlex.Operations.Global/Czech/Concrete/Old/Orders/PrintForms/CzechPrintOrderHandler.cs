using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel;
using System.Text;

using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.MoDi;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.PrintRegional;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Orders.PrintForms
{
    public sealed class CzechPrintOrderHandler : RequestHandler<PrintOrderRequest, StreamResponse>, ICzechAdapted
    {
        private const int ShowFlampLinkServiceId = 1;

        private static readonly Dictionary<int, string> FirmAddressContactTypePlural = new Dictionary<int, string>
            {
                { (int)FirmAddressContactType.Phone, BLResources.Phones },
                { (int)FirmAddressContactType.Fax, BLResources.Faxes },
                { (int)FirmAddressContactType.Email, BLResources.Emails },
                { (int)FirmAddressContactType.Website, BLResources.WebSites },
                { (int)FirmAddressContactType.Icq, BLResources.Icqs },
                { (int)FirmAddressContactType.Skype, BLResources.Skypes },
                { (int)FirmAddressContactType.Other, BLResources.Others }, // other means "jabber"
            };

        private readonly IFinder _finder;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFormatterFactory _formatterFactory;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IFirmRepository _firmAggregateRepository;
        private readonly IUserContext _userContext;

        public CzechPrintOrderHandler(ISubRequestProcessor requestProcessor,
                                      ISecurityServiceUserIdentifier userIdentifierService,
                                      IFormatterFactory formatterFactory,
                                      IFinder finder,
                                      IClientProxyFactory clientProxyFactory,
                                      IFirmRepository firmAggregateRepository,
                                      IUserContext userContext)
        {
            _userIdentifierService = userIdentifierService;
            _formatterFactory = formatterFactory;
            _requestProcessor = requestProcessor;
            _finder = finder;
            _clientProxyFactory = clientProxyFactory;
            _firmAggregateRepository = firmAggregateRepository;
            _userContext = userContext;
        }

        protected override StreamResponse Handle(PrintOrderRequest request)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                    {
                        order.BranchOfficeOrganizationUnitId,
                        order.Number,
                        order.RegionalNumber,
                        order.Currency.ISOCode,
                        order.OwnerCode
                    })
                .Single();

            if (orderInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            if (request.PrintRegionalVersion)
            {
                var streamResponse = PrintRegionalOrder(request.OrderId, orderInfo.RegionalNumber);
                return streamResponse;
            }

            var response = (StreamResponse)_requestProcessor.HandleSubRequest(
                new PrintDocumentRequest
                    {
                        CurrencyIsoCode = orderInfo.ISOCode,
                        FileName = orderInfo.Number,
                        BranchOfficeOrganizationUnitId = orderInfo.BranchOfficeOrganizationUnitId.Value,
                        TemplateCode = GetTemplateCode(request),
                        PrintData = GetPrintData(request, orderInfo.OwnerCode)
                    },
                Context);

            return response;
        }

        private StreamResponse PrintRegionalOrder(long orderId, string orderRegionalNumber)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IPrintRegionalApplicationService, WSHttpBinding>();

            var response = clientProxy.Execute(service => service.PrintRegionalOrder(orderId));
            if (response.Items.Length == 0)
            {
                throw new NotificationException(BLResources.OrderTotalAmountIsZero);
            }

            var streamResponse = new StreamResponse();
            if (response.Items.Length == 1)
            {
                var file = response.Items.First().File;

                streamResponse.FileName = file.FileName;
                streamResponse.ContentType = file.ContentType;
                streamResponse.Stream = new MemoryStream(file.Stream);
                return streamResponse;
            }

            var streamDictionary = response.Items.Select(x => x.File).ToDictionary<FileDescription, string, Stream>(x => x.FileName, x => new MemoryStream(x.Stream));
            streamResponse.FileName = orderRegionalNumber + ".zip";
            streamResponse.ContentType = MediaTypeNames.Application.Zip;
            streamResponse.Stream = streamDictionary.ZipStreamDictionary();
            return streamResponse;
        }

        private TemplateCode GetTemplateCode(PrintOrderRequest request)
        {
            var sourceOrganizationUnitId = _finder.Find(Specs.Find.ById<Order>(request.OrderId)).Select(order => order.SourceOrganizationUnitId).Single();

            var templateCode = GetLocalTemplateCode(sourceOrganizationUnitId);
            return templateCode;
        }

        private TemplateCode GetLocalTemplateCode(long sourceOrganizationUnitId)
        {
            var withVat = GetContributionType(sourceOrganizationUnitId) == ContributionTypeEnum.Branch;
            return withVat ? TemplateCode.OrderWithVatWithDiscount : TemplateCode.OrderWithoutVatWithDiscount;
        }

        private object GetPrintData(PrintOrderRequest request, long ownerCode)
        {
            var orderOwnerName = _userIdentifierService.GetUserInfo(ownerCode).DisplayName;

            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Where(x => x.Id == request.OrderId)
                .Select(x => new
                    {
                        Order = x,
                        OrderExtension = new OrderExtensionDto
                            {
                                PayablePrice = x.PayablePrice,
                                PayablePlan = x.PayablePlan,

                                // нет такой колонки, высчитываем сами
                                Vat = x.PayablePrice * x.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate / 100m,
                                PayablePriceWithVat = x.PayablePrice * (100m + x.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate) / 100m,
                                VatPlan = x.VatPlan,
                                VatRatio = x.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                                DiscountPercent = x.DiscountPercent,
                                DiscountSum = x.DiscountSum,
                            },
                        TerminatedOrder = x.TechnicallyTerminatedOrder,
                        x.Firm,
                        x.LegalPerson,
                        Profile = x.LegalPerson.LegalPersonProfiles.FirstOrDefault(y => y.Id == request.LegalPersonProfileId),
                        MainProfile = x.LegalPerson.LegalPersonProfiles.FirstOrDefault(y => y.Id == request.LegalPersonProfileId && y.IsMainProfile),
                        x.BranchOfficeOrganizationUnit,
                        x.BranchOfficeOrganizationUnit.BranchOffice,
                        CurrencyISOCode = x.Currency.ISOCode,
                        Addresses = x.Firm.FirmAddresses.Where(y => y.IsActive && !y.IsDeleted && !y.ClosedForAscertainment)
                                 .OrderBy(y => y.SortingPosition)
                                 .Select(y => new AddressDto
                                     {
                                         Id = y.Id,
                                         Address = y.Address + ((y.ReferencePoint == null) ? string.Empty : " Ч " + y.ReferencePoint),
                                         WorkingTime = y.WorkingTime,
                                         PaymentMethods = y.PaymentMethods,
                                         ReferencePoint = y.ReferencePoint,
                                         ShowFlampLink =
                                             y.FirmAddressServices.Where(z => z.ServiceId == ShowFlampLinkServiceId).Select(z => (bool?)z.DisplayService).
                                                  FirstOrDefault(),
                                     }),
                        x.Bargain,
                        SourceElectronicMedia = x.SourceOrganizationUnit.ElectronicMedia,
                        SourceBranchOfficeOrganizationUnit =
                                 x.SourceOrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(
                                     y => y.IsActive && !y.IsDeleted && y.IsPrimary),
                        DestBranchOfficeOrganizationUnit =
                                 x.DestOrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(
                                     y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales),
                        DestBranchOffice =
                                 x.DestOrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(
                                     y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales).BranchOffice,
                        x.DestOrganizationUnit.ElectronicMedia,
                        SchedulePayments = x.Bills.Where(y => y.IsActive && !y.IsDeleted)
                                 .OrderBy(y => y.BillDate)
                                 .Select(y => new
                                     {
                                         y.BillNumber,
                                         y.PaymentDatePlan,
                                         y.PayablePlan,
                                     }),
                        OrderPositions = x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted && y.PayablePrice != 0m)
                                 .OrderBy(y => y.Id)
                                 .Select(y => new
                                     {
                                         y.PricePosition.Position.Name,
                                         y.Amount,
                                         y.DiscountSum,
                                         y.DiscountPercent,
                                         y.PayablePrice,
                                         y.PricePerUnit,
                                         y.PayablePlanWoVat,
                                         VatSum = y.PayablePlan - y.PayablePlanWoVat,
                                         Platform = y.PricePosition.Position.Platform.DgppId,
                                         y.PricePosition.Position.IsComposite,

                                         // нет такой колонки, высчитываем сами
                                         PayablePriceWithVat = y.PricePerUnitWithVat * y.ShipmentPlan,
                                         Vat = (y.PricePerUnitWithVat * y.ShipmentPlan) - y.PayablePrice,
                                         PriceForMonthWithDiscount = (y.PayablePlanWoVat / y.Amount) / y.Order.ReleaseCountPlan,
                                         y.PayablePlan,
                                         Advertisements = y.OrderPositionAdvertisements
                                                  .Select(z => new { Category = z.Category.Name, Address = z.FirmAddress.Address + ((z.FirmAddress.ReferencePoint == null) ? string.Empty : " Ч " + z.FirmAddress.ReferencePoint), z.Theme })
                                                  .GroupBy(z => z.Address)
                                                  .Select(z => new
                                                          {
                                                              Address = z.Key,
                                                              Categories = z.Select(p => p.Category) 
                                                                            .Union(z.SelectMany(p => p.Theme.ThemeCategories
                                                                                  .Where(c => !c.IsDeleted && c.Category.IsActive && !c.Category.IsDeleted)
                                                                                  .Select(c => c.Category.Name)))
                                                          })
                                     }),
                        Categories = x.Firm.FirmAddresses.Where(y => y.IsActive && !y.IsDeleted && !y.ClosedForAscertainment)
                                 .OrderBy(y => y.SortingPosition)
                                 .SelectMany(y => y.CategoryFirmAddresses)
                                 .Where(y => y.IsActive && !y.IsDeleted)
                                 .OrderBy(y => y.SortingPosition)
                                 .Select(y => y.Category.Name)
                                 .Distinct(),
                        x.PaymentMethod,
                        OrderSignupDate = x.SignupDate,
                    })
                .Single();
            
            var orderPositions = orderInfo.OrderPositions
                .Select(y =>
                    {
                        var name = y.Name;
                        if (!y.IsComposite)
                        {
                            var bindings = y.Advertisements.Select(z =>
                                {
                                    var categories = z.Categories.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                                    return !categories.Any()
                                                ? z.Address
                                                : string.IsNullOrEmpty(z.Address)
                                                        ? string.Join(", ", categories)
                                                        : string.Format("{0}: {1}", z.Address, string.Join(", ", categories));
                                })
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .ToArray();

                            if (bindings.Any())
                            {
                                name += ": " + string.Join(", ", bindings);
                            }
                        }

                        return new OrderPositionDto
                            {
                                ReleaseCountPlan = orderInfo.Order.ReleaseCountPlan,
                                PayablePlanWithoutVat = y.PayablePlanWoVat,
                                Name = name,
                                FirmName = orderInfo.Firm.Name,
                                Amount = y.Amount,
                                BeginDistributionDate = orderInfo.Order.BeginDistributionDate,
                                PricePerUnit = y.PricePerUnit,
                                DiscountSum = y.DiscountSum,
                                DiscountPercent = y.DiscountPercent.ToString("F"),
                                PayablePrice = y.PayablePrice,
                                PayablePriceWithVat = y.PayablePriceWithVat,
                                PayablePlan = y.PayablePlan,
                                PriceForMonthWithDiscount = y.PriceForMonthWithDiscount,
                                VatSum = y.VatSum,
                                Vat = y.Vat,
                                ElectronicMediaParagraph = GetElectronicMediaParagraph(
                                    (PlatformEnum)y.Platform,
                                    orderInfo.ElectronicMedia,
                                    orderInfo.DestBranchOfficeOrganizationUnit.RegistrationCertificate,
                                    orderInfo.Order)
                            };
                    })
                .ToArray();

            orderInfo.OrderExtension.PayablePlanWithoutVat = orderPositions.Sum(y => y.PayablePlanWithoutVat);
            orderInfo.OrderExtension.VatSum = orderPositions.Sum(y => y.VatSum);

            var profile = orderInfo.MainProfile ?? orderInfo.Profile;
            var legalPersonType = (LegalPersonType)orderInfo.LegalPerson.LegalPersonTypeEnum;

            var personPrefix = GetPersonPrefix(legalPersonType);

            var clientRequisites = string.Format(
                CultureInfo.CurrentCulture,
                GetRequisitesTemplate(legalPersonType),
                orderInfo.LegalPerson.Ic,
                orderInfo.LegalPerson.Inn,
                orderInfo.LegalPerson.LegalAddress,
                profile.AccountNumber,
                profile.BankCode,
                profile.BankName,
                profile.Registered);

            var result = new
                {
                    orderInfo.Order,
                    orderInfo.OrderExtension,
                    OrderPositions = orderPositions,
                    orderInfo.Firm,
                    orderInfo.BranchOfficeOrganizationUnit,
                    orderInfo.BranchOffice,
                    orderInfo.CurrencyISOCode,
                    orderInfo.Bargain,
                    RelatedBargainInfo =
                        orderInfo.Bargain != null
                            ? string.Format(BLResources.RelatedToBargainInfoTemplate,
                                            orderInfo.Bargain.Number,
                                            PrintFormFieldsFormatHelper.FormatLongDate(orderInfo.Bargain.CreatedOn))
                            : null,
                    orderInfo.LegalPerson,
                    Profile = profile,
                    orderInfo.DestBranchOfficeOrganizationUnit,
                    orderInfo.DestBranchOffice,
                    orderInfo.DestBranchOfficeOrganizationUnit.RegistrationCertificate,
                    orderInfo.SourceBranchOfficeOrganizationUnit,
                    orderInfo.SourceElectronicMedia,
                    orderInfo.ElectronicMedia,
                    VatViaPayablePlan = orderInfo.OrderExtension.VatRatio * orderInfo.OrderExtension.PayablePlan / (100m + orderInfo.OrderExtension.VatRatio),
                    orderInfo.SchedulePayments,
                    AdvMatherialsDeadline = GetAdvMatherialsDeadline(orderInfo.Order),
                    OrderOwnerName = orderOwnerName,

                    PriceAllUnitsPerRelease = orderPositions.Sum(position => position.PricePerUnit),

                    // оплата через 5 дней с учЄтом выходных (пока просто суббота\воскресенье)
                    RegionalFranchiseeData = new
                        {
                            PaymentDate = orderInfo.Order.BeginDistributionDate.AddDaysWithDayOffs(4),
                        },

                    NowDate = DateTime.Now,
                    orderInfo.OrderSignupDate,

                    // абзац про техническое расторжение
                    TechnicalTerminationParagraph = GetTechnicalTerminationParagraph(orderInfo.Order, orderInfo.TerminatedOrder, orderInfo.CurrencyISOCode),

                    // строка "действует на основании"
                    OperatesOnTheBasis = GetOperatesOnTheBasisString(profile),
                    FirmAddresses = Enumerable.Range(1, int.MaxValue).Zip(orderInfo.Addresses, CreateFirmAddressPrintData).ToArray(),
                    Categories = FormatCategories(orderInfo.Categories),
                    PaymentMethod = ((PaymentMethod)orderInfo.PaymentMethod).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    PersonPrefix = personPrefix,
                    ClientRequisites = clientRequisites,
                    ClientLegalNamePrefix = GetClientLegalNamePrefixTemplate(legalPersonType)
                };

           return result;
        }

        #region Subroutines

        private FirmAddressPrintData CreateFirmAddressPrintData(int index, AddressDto firmAddressDto)
        {
            // Address
            var addressBuilder = new StringBuilder();

            // address number
            addressBuilder.Append(BLResources.AddressNumber).Append(index).AppendLine(":");
            var address = firmAddressDto.Address + ((firmAddressDto.ReferencePoint == null) ? string.Empty : " Ч " + firmAddressDto.ReferencePoint);
            addressBuilder.AppendLine(address);

            // Contacts
            foreach (var contact in _firmAggregateRepository.GetContacts(firmAddressDto.Id).ToLookup(x => x.ContactType, x => x.Contact))
            {
                var template = FirmAddressContactTypePlural[contact.Key];
                addressBuilder.Append(template).Append(": ").AppendLine(string.Join("; ", contact));
            }

            // WorkingTime
            if (!string.IsNullOrEmpty(firmAddressDto.WorkingTime))
            {
                var localizedWorkingTime = FirmWorkingTimeLocalizer.LocalizeWorkingTime(firmAddressDto.WorkingTime, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                addressBuilder.Append(MetadataResources.WorkingTime).Append(": ").AppendLine(localizedWorkingTime);
            }

            // PaymentMethods
            if (!string.IsNullOrEmpty(firmAddressDto.PaymentMethods))
            {
                addressBuilder.Append(MetadataResources.PaymentMethods).Append(": ").AppendLine(firmAddressDto.PaymentMethods);
            }

            string flampLink;
            switch (firmAddressDto.ShowFlampLink)
            {
                case null:
                    flampLink = null;
                    break;

                case true:
                    flampLink = BLResources.PrintOrderHandler_ShowFlampLinkModeFlampPublished;
                    break;

                case false:
                    flampLink = BLResources.PrintOrderHandler_ShowFlampLinkModeFlampNotPublished;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!string.IsNullOrEmpty(flampLink))
            {
                addressBuilder.Append(MetadataResources.Reviews).Append(": ").AppendLine(flampLink);
            }

            return new FirmAddressPrintData
            {
                FirmAddressInfo = addressBuilder.ToString()
            };
        }

        private ContributionTypeEnum GetContributionType(long organizationUnitId)
        {
            var contributionType = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                .SelectMany(x => x.BranchOfficeOrganizationUnits)
                .Where(x => x.IsActive && !x.IsDeleted && x.IsPrimary)
                .Select(x => x.BranchOffice.ContributionTypeId)
                .SingleOrDefault();

            if (contributionType == null)
            {
                throw new NotificationException(
                    string.Format(CultureInfo.CurrentCulture, BLResources.ContributionTypeIsNotSet, organizationUnitId));
            }

            return (ContributionTypeEnum)contributionType.Value;
        }

        private string GetTechnicalTerminationParagraph(Order order, Order terminatedOrder, int currencyIsoCode)
        {
            if (terminatedOrder == null)
            {
                return BLResources.PrintOrderHandler_TechnicalTerminationParagraph1;
            }

            // order.BeginDistributionDate
            var dateTimeLongDateFormatter = _formatterFactory.Create(typeof(DateTime), FormatType.LongDate, currencyIsoCode);
            var beginDistributionDate = dateTimeLongDateFormatter.Format(order.BeginDistributionDate);

            // terminatedOrder.Number
            var stringUnspecifiedFormatter = _formatterFactory.Create(typeof(string), FormatType.Unspecified, currencyIsoCode);
            var terminatedOrderNumber = stringUnspecifiedFormatter.Format(terminatedOrder.Number);

            // terminatedOrder.SignupDate
            var terminatedOrderSignupDate = dateTimeLongDateFormatter.Format(terminatedOrder.SignupDate);

            // terminatedOrder.EndDistributionDateFact
            var terminatedOrderEndDistributionDateFact = dateTimeLongDateFormatter.Format(terminatedOrder.EndDistributionDateFact);

            return string.Format(
                CultureInfo.CurrentCulture,
                BLResources.PrintOrderHandler_TechnicalTerminationParagraph2,
                beginDistributionDate,
                terminatedOrderNumber,
                terminatedOrderSignupDate,
                terminatedOrderEndDistributionDateFact);
        }

        private static string GetPersonPrefix(LegalPersonType legalPersonType)
        {
            return legalPersonType == LegalPersonType.Businessman
                ? BLResources.CzechPrintOrderHandler_PersonPrefixBusinessman
                : BLResources.CzechPrintOrderHandler_PersonPrefixLegalPerson;
        }

        private static string GetClientLegalNamePrefixTemplate(LegalPersonType legalPersonType)
        {
            return legalPersonType == LegalPersonType.Businessman
                ? BLResources.CzechPrintOrderHandler_ClientLegalNamePrefixBusinessman
                : BLResources.CzechPrintOrderHandler_ClientLegalNamePrefixLegalPerson;
        }

        private static string GetRequisitesTemplate(LegalPersonType legalPersonType)
        {
            return legalPersonType == LegalPersonType.Businessman
                ? BLResources.CzechPrintOrderHandler_ClientRequisitesBusinessman
                : BLResources.CzechPrintOrderHandler_ClientRequisitesLegalPerson;
        }

        private static string GetOperatesOnTheBasisString(LegalPersonProfile profile)
        {
            if (profile.OperatesOnTheBasisInGenitive != (int)OperatesOnTheBasisType.Warranty)
            {
                return string.Empty;
            }

            if (profile.WarrantyBeginDate == null)
            {
                return string.Empty;
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                BLResources.CzechPrintOrderHandler_OperatesOnTheBasisStringTemplate,
                ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                profile.WarrantyBeginDate.Value.ToShortDateString());
        }

        private static string FormatCategories(IEnumerable<string> categories)
        {
            return string.Join("; ", Enumerable.Range(1, int.MaxValue).Zip(categories, (index, category) => string.Format("{0}. {1}", index, category)));
        }

        private static DateTime GetAdvMatherialsDeadline(Order order)
        {
            const int day = 18;
            return order.BeginDistributionDate.AddMonths(-1).AddDays(day - 1);
        }

        private static string GetElectronicMediaParagraph(PlatformEnum platform, string electronivMedia, string registrationCertificate, Order order)
        {
            switch (platform)
            {
                case PlatformEnum.Independent:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLResources.PrintOrderHandler_ElectronicMedaiParagraphIndependent,
                        electronivMedia,
                        registrationCertificate,
                        order.BeginReleaseNumber,
                        order.EndReleaseNumberPlan);
                case PlatformEnum.Desktop:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLResources.PrintOrderHandler_ElectronicMedaiParagraphPC,
                        electronivMedia,
                        registrationCertificate,
                        order.BeginReleaseNumber,
                        order.EndReleaseNumberPlan,
                        PrintFormFieldsFormatHelper.FormatLongDate(order.BeginDistributionDate),
                        PrintFormFieldsFormatHelper.FormatLongDate(order.EndDistributionDatePlan));
                case PlatformEnum.Mobile:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLResources.PrintOrderHandler_ElectronicMedaiParagraphMobile,
                        electronivMedia,
                        registrationCertificate,
                        order.BeginReleaseNumber,
                        order.EndReleaseNumberPlan,
                        PrintFormFieldsFormatHelper.FormatLongDate(order.BeginDistributionDate),
                        PrintFormFieldsFormatHelper.FormatLongDate(order.EndDistributionDatePlan));
                case PlatformEnum.Api:
                    return BLResources.PrintOrderHandler_ElectronicMedaiParagraphApi;
                case PlatformEnum.Online:
                    return BLResources.PrintOrderHandler_ElectronicMedaiParagraphOnline;
                default:
                    throw new ArgumentOutOfRangeException("platform");
            }
        }

        #endregion

        #region nested types

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private sealed class OrderPositionDto
        {
            public string FirmName { get; set; }
            public string Name { get; set; }
            public int Amount { get; set; }
            public DateTime BeginDistributionDate { get; set; }
            public decimal PricePerUnit { get; set; }
            public decimal DiscountSum { get; set; }
            public string DiscountPercent { get; set; }
            public decimal PayablePlanWithoutVat { get; set; }
            public decimal PriceForMonthWithDiscount { get; set; }
            public decimal VatSum { get; set; }

            public string ElectronicMediaParagraph { get; set; }

            // без скидки без Ќƒ—
            public decimal PayablePrice { get; set; }

            // без скидки с Ќƒ—
            public decimal PayablePriceWithVat { get; set; }

            // со скидкой с Ќƒ—
            public decimal PayablePlan { get; set; }

            public decimal Vat { get; set; }
            public int ReleaseCountPlan { get; set; }
        }

        private sealed class OrderExtensionDto
        {
            public decimal PayablePrice { get; set; }
            public decimal PayablePriceWithVat { get; set; }
            public decimal PayablePlanWithoutVat { get; set; }
            public decimal PayablePlan { get; set; }
            public decimal Vat { get; set; }
            public decimal VatSum { get; set; }
            public decimal VatPlan { get; set; }
            public decimal? VatRatio { get; set; }

            public decimal? DiscountPercent { get; set; }
            public decimal? DiscountSum { get; set; }
        }

        private sealed class AddressDto
        {
            public long Id { get; set; }
            public string Address { get; set; }
            public string ReferencePoint { get; set; }
            public string WorkingTime { get; set; }
            public string PaymentMethods { get; set; }
            public bool? ShowFlampLink { get; set; }
        }

        private sealed class FirmAddressPrintData
        {
            public string FirmAddressInfo { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        #endregion
    }
}