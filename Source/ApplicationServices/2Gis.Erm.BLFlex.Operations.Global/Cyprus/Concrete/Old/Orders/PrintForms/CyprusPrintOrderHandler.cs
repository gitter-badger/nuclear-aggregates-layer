using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel;
using System.Text;

using DoubleGis.Erm.BL.Aggregates.Firms;
using DoubleGis.Erm.BL.API.Common.Enums;
using DoubleGis.Erm.BL.API.MoDi;
using DoubleGis.Erm.BL.API.MoDi.Remote.PrintRegional;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Operations.Crosscutting;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Concrete.Old.Orders.PrintForms
{
    public sealed class CyprusPrintOrderHandler : RequestHandler<PrintOrderRequest, StreamResponse>, ICyprusAdapted
    {
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

        public CyprusPrintOrderHandler(ISubRequestProcessor requestProcessor,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFormatterFactory formatterFactory,
            IFinder finder,
            IClientProxyFactory clientProxyFactory, 
            IFirmRepository firmAggregateRepository)
        {
            _userIdentifierService = userIdentifierService;
            _formatterFactory = formatterFactory;
            _requestProcessor = requestProcessor;
            _finder = finder;
            _clientProxyFactory = clientProxyFactory;
            _firmAggregateRepository = firmAggregateRepository;
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

                    // TODO. В этом объекте уже нет нужды, в связи с удалением OrderExtensions, надо его удалить 
                    // и поправить хренову тучу печатных форм :/.
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
                    x.BranchOfficeOrganizationUnit,
                    x.BranchOfficeOrganizationUnit.BranchOffice,
                    CurrencyISOCode = x.Currency.ISOCode,
                    Addresses = x.Firm.FirmAddresses.Where(y => y.IsActive && !y.IsDeleted && !y.ClosedForAscertainment)
                        .OrderBy(y => y.SortingPosition)
                        .Select(y => new AddressDto
                        {
                            Id = y.Id,
                            Address = y.Address + ((y.ReferencePoint == null) ? string.Empty : " — " + y.ReferencePoint),
                            WorkingTime = y.WorkingTime,
                            PaymentMethods = y.PaymentMethods,
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
                            .Select(z => new { Category = z.Category.Name, Address = z.FirmAddress.Address + ((z.FirmAddress.ReferencePoint == null) ? string.Empty : " — " + z.FirmAddress.ReferencePoint), z.Theme })
                            .GroupBy(z => z.Address)
                            .Select(
                                z =>
                                new
                                {
                                    Address = z.Key,
                                    Categories =
                                        z.Select(p => p.Category)
                                        .Union(z.SelectMany(p =>
                                            p.Theme.ThemeCategories
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
                        x.PaymentMethod
                })
                .AsEnumerable()

                // in-memory transformations
                .Select(x =>
                {
                    var orderPositions = x.OrderPositions
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
                                                  ReleaseCountPlan = x.Order.ReleaseCountPlan,
                                                  PayablePlanWithoutVat = y.PayablePlanWoVat,
                                                  Name = name,
                                                  FirmName = x.Firm.Name,
                                                  Amount = y.Amount,
                                                  BeginDistributionDate = x.Order.BeginDistributionDate,
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
                                                      x.ElectronicMedia,
                                                      x.DestBranchOfficeOrganizationUnit.RegistrationCertificate,
                                                      x.Order)
                                              };
                                          })
                                          .ToArray();

                    x.OrderExtension.PayablePlanWithoutVat = orderPositions.Sum(y => y.PayablePlanWithoutVat);
                    x.OrderExtension.VatSum = orderPositions.Sum(y => y.VatSum);

                    return new
                    {
                        x.Order,
                        x.OrderExtension,
                        OrderPositions = orderPositions,
                        x.Firm,
                        x.BranchOfficeOrganizationUnit,
                        x.BranchOffice,
                        x.CurrencyISOCode,
                        x.Bargain,
                        RelatedBargainInfo =
                            x.Bargain != null
                                ? string.Format(BLResources.RelatedToBargainInfoTemplate, x.Bargain.Number, PrintFormFieldsFormatHelper.FormatLongDate(x.Bargain.CreatedOn))
                                : null,
                        x.LegalPerson,
                        x.Profile,
                        LegalPersonAddress =
                            (x.Profile != null && x.Profile.DocumentsDeliveryMethod == (int)DocumentsDeliveryMethod.DeliveryByManager)
                                ? x.Profile.DocumentsDeliveryAddress
                                : (x.Profile != null && x.Profile.DocumentsDeliveryMethod == (int)DocumentsDeliveryMethod.PostOnly)
                                      ? x.Profile.PostAddress
                                      : null,
                        x.DestBranchOfficeOrganizationUnit,
                        x.DestBranchOffice,
                        x.DestBranchOfficeOrganizationUnit.RegistrationCertificate,
                        x.SourceBranchOfficeOrganizationUnit,
                        x.SourceElectronicMedia,
                        x.ElectronicMedia,
                        VatViaPayablePlan = x.OrderExtension.VatRatio * x.OrderExtension.PayablePlan / (100m + x.OrderExtension.VatRatio),
                        x.SchedulePayments,
                        AdvMatherialsDeadline = GetAdvMatherialsDeadline(x.Order),
                        OrderOwnerName = orderOwnerName,

                        PriceAllUnitsPerRelease = orderPositions.Sum(position => position.PricePerUnit),

                        // оплата через 5 дней с учётом выходных (пока просто суббота\воскресенье)
                        RegionalFranchiseeData = new
                        {
                            PaymentDate = x.Order.BeginDistributionDate.AddDaysWithDayOffs(4),
                        },

                        // В кипрской версии не нужна информация о скидке.
                        DiscountInfo = string.Empty,
                        NowDate = DateTime.Now,

                        // абзац про заключение договора
                        BeginContractParagraph = GetBeginContractParagraph(x.BranchOfficeOrganizationUnit, x.LegalPerson, x.Profile),

                        // абзац про техническое расторжение
                        TechnicalTerminationParagraph = GetTechnicalTerminationParagraph(x.Order, x.TerminatedOrder, x.CurrencyISOCode),

                        // абзац про платёжные реквизиты
                        ClientRequisitesParagraph = GetClientRequisitesParagraph(x.LegalPerson, x.Profile),
                        Addresses = Enumerable.Range(1, int.MaxValue).Zip(x.Addresses, (index, address) => new { Address = FormatAddress(index, address) }),
                        Categories = FormatCategories(x.Categories),
                        PaymentMethod = ((PaymentMethod)x.PaymentMethod).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)
                    };
                })
                .Single();

            // task 3818 нужно доработать
            // AddOrSetJournalProperty<List<int>>(JournalMakeRegionalAdsDocsProperties.OrderIds, orderInfo.Order.Id);
            // AddOrSetJournalProperty<decimal?>(JournalMakeRegionalAdsDocsProperties.TotalAmount, orderInfo.OrderExtension.PayablePlan);
            return orderInfo;
        }

        #region Subroutines

        private static string FormatCategories(IEnumerable<string> categories)
        {
            return string.Join("; ", Enumerable.Range(1, int.MaxValue).Zip(categories, (index, category) => string.Format("{0}. {1}", index, category)));
        }

        private string FormatAddress(int addressNumber, AddressDto address)
        {
            var stringBuilder = new StringBuilder();

            // address number
            stringBuilder.Append(BLResources.AddressNumber).Append(addressNumber).AppendLine(":");
            stringBuilder.AppendLine(address.Address);

            // Contacts
            foreach (var contact in _firmAggregateRepository.GetContacts(address.Id).ToLookup(x => x.ContactType, x => x.Contact))
            {
                var template = FirmAddressContactTypePlural[contact.Key];
                stringBuilder.Append(template).Append(": ").AppendLine(string.Join("; ", contact));
            }

            // WorkingTime
            if (!string.IsNullOrEmpty(address.WorkingTime))
            {
                var localizedWorkingTime = FirmWorkingTimeLocalizer.LocalizeWorkingTime(address.WorkingTime, CultureInfo.CurrentCulture);
                stringBuilder.Append(MetadataResources.WorkingTime).Append(": ").AppendLine(localizedWorkingTime);
            }

            // PaymentMethods
            if (!string.IsNullOrEmpty(address.PaymentMethods))
            {
                stringBuilder.Append(MetadataResources.PaymentMethods).Append(": ").AppendLine(address.PaymentMethods);                
            }

            return stringBuilder.ToString();
        }

        private static DateTime GetAdvMatherialsDeadline(Order order)
        {
            const int day = 18;
            return order.BeginDistributionDate.AddMonths(-1).AddDays(day - 1);
        }

        private string GetDiscountInfo(decimal? discountPercent, decimal? discountSum, int currencyIsoCode)
        {
            if (discountSum == null || discountSum.Value == 0m || discountPercent == null || discountPercent.Value == 0m)
            {
                return null;
            }

            // discount percent (number formatter)
            var decimalPercentsFormatter = _formatterFactory.Create(typeof(decimal), FormatType.Percents, currencyIsoCode);
            var discountPercentNumber = decimalPercentsFormatter.Format(discountPercent);

            // discount sum (money formatter)
            var decimalMoneyFormatter = _formatterFactory.Create(typeof(decimal), FormatType.Money, currencyIsoCode);
            var discountSumMoney = decimalMoneyFormatter.Format(discountSum);

            // discount sum (money words formatter)
            var decimalMoneyWordsFormatter = _formatterFactory.Create(typeof(decimal), FormatType.MoneyWords, currencyIsoCode);
            var discountSumMoneyWords = decimalMoneyWordsFormatter.Format(discountSum);

            return string.Format(CultureInfo.CurrentCulture, BLResources.PrintOrderHandler_DiscountInfo, discountPercentNumber, discountSumMoney, discountSumMoneyWords);
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

        private static string GetBeginContractParagraph(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit, LegalPerson legalPerson, LegalPersonProfile profile)
        {
            var operatesOnTheBasisInGenitive = string.Empty;
            switch ((LegalPersonType)legalPerson.LegalPersonTypeEnum)
            {
                case LegalPersonType.NaturalPerson:

                    if (profile != null && profile.OperatesOnTheBasisInGenitive != null)
                    {
                        switch ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive)
                        {
                            case OperatesOnTheBasisType.Underfined:
                                operatesOnTheBasisInGenitive = string.Empty;
                                break;
                            case OperatesOnTheBasisType.Warranty:
                                operatesOnTheBasisInGenitive = string.Format(
                                    BLResources.OperatesOnBasisOfWarantyTemplateForNaturalPerson,
                                    ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                    profile.WarrantyNumber,
                                    PrintFormFieldsFormatHelper.FormatShortDate(profile.WarrantyBeginDate));
                                break;
                            default:
                                throw new BusinessLogicDataException((LegalPersonType)legalPerson.LegalPersonTypeEnum, (OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive);
                        }
                    }

                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLResources.PrintOrderHandler_BeginContractParagraph1,
                        branchOfficeOrganizationUnit.ShortLegalName,
                        branchOfficeOrganizationUnit.PositionInGenitive,
                        branchOfficeOrganizationUnit.ChiefNameInGenitive,
                        branchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive,
                        legalPerson.LegalName,
                        operatesOnTheBasisInGenitive);

                case LegalPersonType.Businessman:
                case LegalPersonType.LegalPerson:
                    {
                        if (profile != null && profile.OperatesOnTheBasisInGenitive != null)
                        {
                            switch ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive)
                            {
                                case OperatesOnTheBasisType.Underfined:
                                    operatesOnTheBasisInGenitive =
                                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture);
                                    break;
                                case OperatesOnTheBasisType.Charter:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLResources.OperatesOnBasisOfCharterTemplate,
                                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                                    break;
                                case OperatesOnTheBasisType.Certificate:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLResources.OperatesOnBasisOfCertificateTemplate,
                                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                        profile.CertificateNumber,
                                        PrintFormFieldsFormatHelper.FormatShortDate(profile.CertificateDate));
                                    break;
                                case OperatesOnTheBasisType.Warranty:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLResources.OperatesOnBasisOfWarantyTemplate,
                                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                        profile.WarrantyNumber,
                                        PrintFormFieldsFormatHelper.FormatShortDate(profile.WarrantyBeginDate));
                                    break;
                                case OperatesOnTheBasisType.Bargain:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLResources.OperatesOnBasisOfBargainTemplate,
                                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                        profile.BargainNumber,
                                        PrintFormFieldsFormatHelper.FormatShortDate(profile.BargainBeginDate));
                                    break;
                                case OperatesOnTheBasisType.FoundingBargain:
                                    operatesOnTheBasisInGenitive = string.Format(
                                        BLResources.OperatesOnBasisOfFoundingBargainTemplate,
                                        ((OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                                    break;
                                default:
                                    throw new BusinessLogicDataException((LegalPersonType)legalPerson.LegalPersonTypeEnum, (OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive);
                            }
                        }

                        return string.Format(
                            CultureInfo.CurrentCulture,
                            BLResources.PrintOrderHandler_BeginContractParagraph2,
                            branchOfficeOrganizationUnit.ShortLegalName,
                            branchOfficeOrganizationUnit.PositionInGenitive,
                                         branchOfficeOrganizationUnit.ChiefNameInGenitive,
                                         branchOfficeOrganizationUnit.OperatesOnTheBasisInGenitive,
                            legalPerson.LegalName,
                            profile != null ? profile.PositionInGenitive : string.Empty,
                            profile != null ? profile.ChiefNameInGenitive : string.Empty,
                            operatesOnTheBasisInGenitive);
                    }

                default:
                    throw new BusinessLogicDataException((LegalPersonType)legalPerson.LegalPersonTypeEnum);
            }
        }

        private static string GetClientRequisitesParagraph(LegalPerson legalPerson, LegalPersonProfile profile)
        {
            switch ((LegalPersonType)legalPerson.LegalPersonTypeEnum)
            {
                case LegalPersonType.NaturalPerson:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLResources.CyprusPrintOrderHandler_ClientRequisitesParagraph1,
                        legalPerson.LegalName,
                        legalPerson.PassportSeries,
                        legalPerson.PassportNumber,
                        legalPerson.PassportIssuedBy,
                        legalPerson.RegistrationAddress);
                case LegalPersonType.Businessman:
                case LegalPersonType.LegalPerson:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLResources.CyprusPrintOrderHandler_ClientRequisitesParagraph2,
                        legalPerson.LegalName,
                        legalPerson.Inn,
                        legalPerson.VAT,
                        legalPerson.LegalAddress,
                        profile != null ? profile.IBAN : string.Empty,
                        profile != null ? profile.SWIFT : string.Empty,
                        profile != null ? profile.AccountNumber : string.Empty,
                        profile != null ? profile.BankName : string.Empty,
                        profile != null ? profile.PaymentEssentialElements : string.Empty);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        private sealed class BusinessLogicDataException : BusinessLogicException
        {
            public BusinessLogicDataException(LegalPersonType legalPerson, OperatesOnTheBasisType operatesOnTheBasis)
                : base(GenerateMessage(legalPerson, operatesOnTheBasis))
            {
            }

            public BusinessLogicDataException(LegalPersonType legalPerson)
                : base(GenerateMessage(legalPerson))
            {
            }

            private static string GenerateMessage(LegalPersonType legalPerson)
            {
                return string.Format(BLResources.CannotPrintOrderForLegalPerson,
                                     legalPerson.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
            }

            private static string GenerateMessage(LegalPersonType legalPerson, OperatesOnTheBasisType operatesOnTheBasis)
            {
                return string.Format(BLResources.CannotPrintOrderForLegalPersonMainDocument,
                                     legalPerson.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                                     operatesOnTheBasis.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture).ToLower());
            }
        }

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

            // без скидки без НДС
            public decimal PayablePrice { get; set; }

            // без скидки с НДС
            public decimal PayablePriceWithVat { get; set; }

            // со скидкой с НДС
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
            public string WorkingTime { get; set; }
            public string PaymentMethods { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        #endregion
    }
}