using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using BLCoreResources = DoubleGis.Erm.BLCore.Resources.Server.Properties.BLResources;
using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    public sealed class PrintOrderHelper
    {
        private static readonly Dictionary<FirmAddressContactType, string> FirmAddressContactTypePlural = new Dictionary<FirmAddressContactType, string>
            {
                { FirmAddressContactType.Phone, BLResources.Phones },
                { FirmAddressContactType.Fax, BLResources.Faxes },
                { FirmAddressContactType.Email, BLResources.Emails },
                { FirmAddressContactType.Website, BLResources.WebSites },
                { FirmAddressContactType.Icq, BLResources.Icqs },
                { FirmAddressContactType.Skype, BLResources.Skypes },
                { FirmAddressContactType.Other, BLResources.Others }, // other means "jabber"
            };

        private readonly IFormatter _longDateFormatter;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public PrintOrderHelper(IFormatterFactory formatterFactory, ISecurityServiceUserIdentifier userIdentifierService)
        {
            _userIdentifierService = userIdentifierService;
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
        }

        public static DateTime GetAdvMatherialsDeadline(DateTime beginDistributionDate)
        {
            const int Day = 18;
            return beginDistributionDate.AddMonths(-1).AddDays(Day - 1);
        }

        public string FormatAddress(int addressNumber, AddressDto address, IEnumerable<FirmContact> contacts, CultureInfo targetCulture)
        {
            var stringBuilder = new StringBuilder();

            // address number
            stringBuilder.Append(BLResources.AddressNumber).Append(addressNumber).AppendLine(":");
            stringBuilder.AppendLine(FormatAddressWithReferencePoint(address.Address, address.ReferencePoint));

            // Contacts
            foreach (var contact in contacts)
            {
                var template = FirmAddressContactTypePlural[contact.ContactType];
                stringBuilder.Append(template).Append(": ").AppendLine(string.Join("; ", contact.Contact));
            }

            // WorkingTime
            if (!string.IsNullOrEmpty(address.WorkingTime))
            {
                var localizedWorkingTime = FirmWorkingTimeLocalizer.LocalizeWorkingTime(address.WorkingTime, targetCulture);
                stringBuilder.Append(MetadataResources.WorkingTime).Append(": ").AppendLine(localizedWorkingTime);
            }

            // PaymentMethods
            if (!string.IsNullOrEmpty(address.PaymentMethods))
            {
                stringBuilder.Append(MetadataResources.PaymentMethods).Append(": ").AppendLine(address.PaymentMethods);
            }

            return stringBuilder.ToString();
        }

        public string GetElectronicMediaParagraph(PlatformEnum platform, string electronivMedia, string registrationCertificate, Order order)
        {
            switch (platform)
            {
                case PlatformEnum.Independent:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLFlexResources.PrintOrderHandler_ElectronicMedaiParagraphIndependent,
                        electronivMedia,
                        registrationCertificate,
                        order.BeginReleaseNumber,
                        order.EndReleaseNumberPlan,
                        _longDateFormatter.Format(order.BeginDistributionDate),
                        _longDateFormatter.Format(order.EndDistributionDatePlan));
                case PlatformEnum.Desktop:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLFlexResources.PrintOrderHandler_ElectronicMedaiParagraphPC,
                        electronivMedia,
                        registrationCertificate,
                        order.BeginReleaseNumber,
                        order.EndReleaseNumberPlan,
                        _longDateFormatter.Format(order.BeginDistributionDate),
                        _longDateFormatter.Format(order.EndDistributionDatePlan));
                case PlatformEnum.Mobile:
                    return string.Format(
                        CultureInfo.CurrentCulture,
                        BLFlexResources.PrintOrderHandler_ElectronicMedaiParagraphMobile,
                        electronivMedia,
                        registrationCertificate,
                        order.BeginReleaseNumber,
                        order.EndReleaseNumberPlan,
                        _longDateFormatter.Format(order.BeginDistributionDate),
                        _longDateFormatter.Format(order.EndDistributionDatePlan));
                case PlatformEnum.Api:
                    return string.Format(BLFlexResources.PrintOrderHandler_ElectronicMedaiParagraphApi, electronivMedia);
                case PlatformEnum.Online:
                    return string.Format(BLFlexResources.PrintOrderHandler_ElectronicMedaiParagraphOnline, electronivMedia);
                default:
                    throw new ArgumentOutOfRangeException("platform");
            }
        }

        public PrintData GetOrderPositionsWithDetailedName(IQueryable<Order> orderQuery, IQueryable<OrderPosition> orderPositionsQuery, SalesModel? salesModel)
        {
            var orderInfo = orderQuery
                .Select(order => new
                {
                    FirmName = order.Firm.Name,
                    order.DestOrganizationUnit.ElectronicMedia,
                    order.DestOrganizationUnit.BranchOfficeOrganizationUnits
                         .FirstOrDefault(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales)
                         .RegistrationCertificate,
                    Order = order,
                })
                .Single();

            var orderPositionNameFormat = salesModel == SalesModel.MultiPlannedProvision
                                              ? PositionDetailedName.FormatMultiFullHouse
                                              : PositionDetailedName.FormatOldSalesModels;

            var orderPositions = orderPositionsQuery
                .Select(orderPosition => new
                    {
                        orderPosition.Amount,
                        orderPosition.DiscountPercent,
                        orderPosition.PricePosition.Position.Name,
                        orderPosition.PayablePlan,
                        orderPosition.PayablePlanWoVat,
                        orderPosition.PricePerUnit,
                        orderPosition.Order.ReleaseCountPlan,
                        PricePositionCost = orderPosition.PricePosition.Cost,

                        Platform = orderPosition.PricePosition.Position.Platform.DgppId,

                        Key = new PositionDetailedName.Key
                                  {
                                      OrderPositionName = orderPosition.PricePosition.Position.Name,
                                      IsComposite = orderPosition.PricePosition.Position.IsComposite,
                                  },

                        Values = orderPosition.OrderPositionAdvertisements
                                  .Select(adv => new PositionDetailedName.Advertisement
                                  {
                                      PositionName = adv.Position.Name,
                                      BindingType = adv.Position.BindingObjectTypeEnum,

                                      Address = adv.FirmAddress.Address,
                                      ReferencePoint = adv.FirmAddress.ReferencePoint,
                                      CategoryName = adv.Category.Name,
                                      ThemeName = adv.Theme.Name,
                                  })
                    })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "AdvertisingBudget", x.PricePositionCost * x.Amount },
                        { "Amount", x.Amount },
                        { "BeginDistributionDate", orderInfo.Order.BeginDistributionDate },
                        { "DiscountPercent", x.DiscountPercent },
                        { "ElectronicMediaParagraph", GetElectronicMediaParagraph((PlatformEnum)x.Platform, orderInfo.ElectronicMedia, orderInfo.RegistrationCertificate, orderInfo.Order) },
                        { "FirmName", orderInfo.FirmName },
                        { "Name", orderPositionNameFormat.Invoke(x.Key, x.Values) },
                        { "PayablePlan", x.PayablePlan },
                        { "PayablePlanWithoutVat", x.PayablePlanWoVat },
                        { "PriceForMonthWithDiscount", (x.PayablePlanWoVat / x.Amount) / x.ReleaseCountPlan },
                        { "PricePerUnit", x.PricePerUnit },
                        { "ReleaseCountPlan", x.ReleaseCountPlan },
                        { "VatSum", x.PayablePlan - x.PayablePlanWoVat },
                    });

            return new PrintData { { "OrderPositions", orderPositions } };
        }

        public PrintData GetOrderPositions(IQueryable<Order> orderQuery, IQueryable<OrderPosition> orderPositionsQuery)
        {
            var orderInfo = orderQuery
                .Select(order => new
                {
                    FirmName = order.Firm.Name,
                    order.DestOrganizationUnit.ElectronicMedia,
                    order.DestOrganizationUnit.BranchOfficeOrganizationUnits
                         .FirstOrDefault(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales)
                         .RegistrationCertificate,
                    Order = order
                })
                .Single();

            var orderPositions = orderPositionsQuery
                .Select(orderPosition => new
                {
                    orderPosition.Amount,
                    orderPosition.DiscountPercent,
                    orderPosition.PricePosition.Position.Name,
                    orderPosition.PayablePlan,
                    orderPosition.PayablePlanWoVat,
                    orderPosition.PricePerUnit,
                    orderPosition.Order.ReleaseCountPlan,
                    orderPosition.PricePosition.Position.IsComposite,
                    BindingObjectTypeEnum = orderPosition.PricePosition.Position.BindingObjectTypeEnum,

                    Platform = orderPosition.PricePosition.Position.Platform.DgppId,

                    Advertisements = orderPosition.OrderPositionAdvertisements
                                                  .Select(z => new
                                                  {
                                                      z.Category.Name,
                                                      z.FirmAddress.ReferencePoint,
                                                      z.FirmAddress.Address,
                                                      ThemeCategoryNames = z.Theme
                                                                            .ThemeCategories
                                                                            .Where(c => !c.IsDeleted && c.Category.IsActive && !c.Category.IsDeleted)
                                                                            .Select(c => c.Category.Name)
                                                  })
                                                  .GroupBy(z => new { z.Address, z.ReferencePoint })
                                                  .Select(z => new PositionSimpleName.AdvertisementDto
                                                  {
                                                      Address = z.Key.Address,
                                                      ReferencePoint = z.Key.ReferencePoint,
                                                      Categories = z.Select(category => category.Name).Union(z.SelectMany(p => p.ThemeCategoryNames)),
                                                  })
                })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "Amount", x.Amount },
                        { "BeginDistributionDate", orderInfo.Order.BeginDistributionDate },
                        { "DiscountPercent", x.DiscountPercent.ToString("F") }, // TODO {all, 12.03.2014}: Форматирования данных в коде не должно быть
                        { "ElectronicMediaParagraph", GetElectronicMediaParagraph((PlatformEnum)x.Platform, orderInfo.ElectronicMedia, orderInfo.RegistrationCertificate, orderInfo.Order) },
                        { "FirmName", orderInfo.FirmName },

                        { "Name", PositionSimpleName.FormatName(x.IsComposite, x.BindingObjectTypeEnum, x.Name, x.Advertisements) },
                        { "PayablePlan", x.PayablePlan },
                        { "PayablePlanWithoutVat", x.PayablePlanWoVat },
                        { "PriceForMonthWithDiscount", (x.PayablePlanWoVat / x.Amount) / x.ReleaseCountPlan },
                        { "PricePerUnit", x.PricePerUnit },
                        { "ReleaseCountPlan", x.ReleaseCountPlan },
                        { "VatSum", x.PayablePlan - x.PayablePlanWoVat },
                    });

            return new PrintData { { "OrderPositions", orderPositions } };
        }

        public PrintData GetFirmAddresses(IQueryable<FirmAddress> query, IDictionary<long, IEnumerable<FirmContact>> contacts, CultureInfo targetCulture)
        {
            var addresses = query
                .Select(y => new AddressDto
                {
                    Id = y.Id,
                    Address = y.Address,
                    ReferencePoint = y.ReferencePoint,
                    WorkingTime = y.WorkingTime,
                    PaymentMethods = y.PaymentMethods,
                })
                .ToArray();

            var printAddresses = Enumerable.Range(1, int.MaxValue)
                .Zip(addresses, (index, address) => new PrintData { { "FirmAddressInfo", FormatAddress(index, address, contacts[address.Id], targetCulture) } })
                .ToArray();

            return new PrintData
                {
                    { "FirmAddresses", printAddresses },
                };
        }

        public PrintData GetPaymentSchedule(IQueryable<Bill> query)
        {
            var payments = query
                .Select(y => new
                {
                    y.PaymentDatePlan,
                    y.PayablePlan
                })
                .AsEnumerable()
                .Select(y => new PrintData
                    {
                        { "PaymentDatePlan", y.PaymentDatePlan },
                        { "PayablePlan", y.PayablePlan }
                    });

            return new PrintData
                {
                    { "SchedulePayments", payments }
                };
        }

        public PrintData GetOrder(IQueryable<Order> query)
        {
            var order = query
                .Select(x => new
                    {
                        x.Number,
                        x.BeginDistributionDate,
                        x.SignupDate,
                        x.OwnerCode,
                        x.PayablePlan,
                        x.VatPlan,
                        x.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                        DiscountPercent = x.DiscountPercent.HasValue ? x.DiscountPercent.Value : 0,
                        PayablePlanWithoutVat = x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted).Select(position => position.PayablePlanWoVat),
                    })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "Number", x.Number },
                        { "BeginDistributionDate", x.BeginDistributionDate },
                        { "SignupDate", x.SignupDate },
                        { "OwnerName", _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName },
                        { "PayablePlan", x.PayablePlan },
                        { "PayablePlanWithoutVat", x.PayablePlanWithoutVat.Sum() },
                        { "VatPlan", x.VatPlan },
                        { "VatRatio", x.VatRate },
                        { "VatSum", x.PayablePlan - x.PayablePlanWithoutVat.Sum() },
                        { "DiscountPercent", x.DiscountPercent },

                        { "UseWithVat", x.VatPlan > 0 },
                        { "UseNoVat", x.VatPlan == 0 },
                        { "UseWithDiscount", x.DiscountPercent > 0 },

                        { "UseWithVatWithDiscount", x.VatPlan > 0 && x.DiscountPercent > 0 },
                        { "UseWithVatNoDiscount", x.VatPlan > 0 && x.DiscountPercent == 0 },
                        { "UseNoVatWithDiscount", x.VatPlan == 0 && x.DiscountPercent > 0 },
                        { "UseNoVatNoDiscount", x.VatPlan == 0 && x.DiscountPercent == 0 },
                    })
                .Single();

            return order;
        }

        public PrintData GetCategories(IQueryable<Order> query)
        {
            return query
                .Select(order => new
                    {
                        Categories = order.Firm.FirmAddresses.Where(y => y.IsActive && !y.IsDeleted && !y.ClosedForAscertainment)
                                          .OrderBy(y => y.SortingPosition)
                                          .SelectMany(y => y.CategoryFirmAddresses)
                                          .Where(y => y.IsActive && !y.IsDeleted)
                                          .OrderBy(y => y.SortingPosition)
                                          .Select(y => y.Category.Name)
                                          .Distinct(),
                    })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "Categories", FormatCategories(x.Categories) },
                    })
                .Single();
        }

        private static string FormatCategories(IEnumerable<string> categories)
        {
            return string.Join("; ", Enumerable.Range(1, int.MaxValue).Zip(categories, (index, category) => string.Format("{0}. {1}", index, category)));
        }

        private static string FormatAddressWithReferencePoint(string address, string referencePoint)
        {
            return string.IsNullOrWhiteSpace(referencePoint)
                       ? address
                       : string.Format(BLFlexResources.AddressWithReferencePoint, address, referencePoint);
        }

        public sealed class AddressDto
        {
            public long Id { get; set; }
            public string Address { get; set; }
            public string ReferencePoint { get; set; }
            public string WorkingTime { get; set; }
            public string PaymentMethods { get; set; }
        }
    }
}
