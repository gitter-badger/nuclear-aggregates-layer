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

        public string FormatAddress(int addressNumber, AddressDto address, IEnumerable<FirmContact> contacts)
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
                        order.EndReleaseNumberPlan);
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
                    BindingObjectTypeEnum = (PositionBindingObjectType)orderPosition.PricePosition.Position.BindingObjectTypeEnum,

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
                                                  .Select(z => new AdvertisementDto
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
                        { "Name", FormatName(x.IsComposite, x.BindingObjectTypeEnum, x.Name, x.Advertisements) },
                        { "PayablePlan", x.PayablePlan },
                        { "PayablePlanWithoutVat", x.PayablePlanWoVat },
                        { "PriceForMonthWithDiscount", (x.PayablePlanWoVat / x.Amount) / x.ReleaseCountPlan },
                        { "PricePerUnit", x.PricePerUnit },
                        { "ReleaseCountPlan", x.ReleaseCountPlan },
                        { "VatSum", x.PayablePlan - x.PayablePlanWoVat },
                    });

            return new PrintData { { "OrderPositions", orderPositions } };
        }

        public PrintData GetFirmAddresses(IQueryable<FirmAddress> query, IDictionary<long, IEnumerable<FirmContact>> contacts)
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
                .Zip(addresses, (index, address) => new PrintData { { "FirmAddressInfo", FormatAddress(index, address, contacts[address.Id]) } })
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
                        x.SignupDate,
                        x.OwnerCode,
                        x.PayablePlan,
                        x.VatPlan,
                        x.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                        PayablePlanWithoutVat = x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted).Select(position => position.PayablePlanWoVat),
                    })
                .AsEnumerable()
                .Select(x => new PrintData
                    {
                        { "Number", x.Number },
                        { "SignupDate", x.SignupDate },
                        { "OwnerName", _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName },
                        { "PayablePlan", x.PayablePlan },
                        { "PayablePlanWithoutVat", x.PayablePlanWithoutVat.Sum() },
                        { "VatPlan", x.VatPlan },
                        { "VatRatio", x.VatRate },
                        { "VatSum", x.PayablePlan - x.PayablePlanWithoutVat.Sum() },
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
                       : address + " — " + referencePoint;
        }

        public string FormatName(bool isPositionComposite, PositionBindingObjectType bindingType, string positionName, IEnumerable<AdvertisementDto> advertisements)
        {
            if (isPositionComposite)
            {
                if (CategoryBindedCompositePosition(bindingType))
                {
                    var categories = advertisements.SelectMany(z => z.Categories)
                                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                                   .Distinct()
                                                   .ToArray();
                    return categories.Any()
                               ? string.Format(BLFlexResources.OrderPositionNameWithContextCategory, positionName, string.Join(", ", categories))
                               : positionName;
                }

                return positionName;
            }

            var bindings = advertisements
                .Select(z =>
                    {
                        var address = FormatAddressWithReferencePoint(z.Address, z.ReferencePoint);
                        var categories = z.Categories.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                        return !categories.Any()
                                   ? address
                                   : string.IsNullOrEmpty(address)
                                         ? string.Join(", ", categories)
                                         : string.Format("{0}: {1}", address, string.Join(", ", categories));
                    })
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            return bindings.Any()
                       ? positionName + ": " + string.Join(", ", bindings)
                       : positionName;
        }

        private bool CategoryBindedCompositePosition(PositionBindingObjectType bindingType)
        {
            var categoryBindedTypes = new[]
            {
                PositionBindingObjectType.CategoryMultipleAsterix,
                PositionBindingObjectType.AddressCategorySingle,
                PositionBindingObjectType.AddressCategoryMultiple,
                PositionBindingObjectType.CategorySingle,
                PositionBindingObjectType.CategoryMultiple,
                PositionBindingObjectType.AddressFirstLevelCategorySingle,
                PositionBindingObjectType.AddressFirstLevelCategoryMultiple,
            };
            return categoryBindedTypes.Contains(bindingType);
        }

        public sealed class AddressDto
        {
            public long Id { get; set; }
            public string Address { get; set; }
            public string ReferencePoint { get; set; }
            public string WorkingTime { get; set; }
            public string PaymentMethods { get; set; }
        }

        public sealed class AdvertisementDto
        {
            public IEnumerable<string> Categories { get; set; }
            public string Address { get; set; }
            public string ReferencePoint { get; set; }
        }


    }
}
