using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.MoDi;
using DoubleGis.Erm.BLCore.API.MoDi.Dto;
using DoubleGis.Erm.BLCore.API.MoDi.Enums;
using DoubleGis.Erm.BLCore.API.MoDi.PrintRegional;
using DoubleGis.Erm.BLCore.API.MoDi.Settings;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.MoDi
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class PrintRegionalOrderService : IPrintRegionalOrderService
    {
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly IPrintFormService _printFormService;
        private readonly IMoneyDistributionSettings _moneyDistributionSettings;

        public PrintRegionalOrderService(IFinder finder, IPrintFormService printFormService, IMoneyDistributionSettings moneyDistributionSettings, IUseCaseTuner useCaseTuner, IUserContext userContext)
        {
            _finder = finder;
            _printFormService = printFormService;
            _moneyDistributionSettings = moneyDistributionSettings;
            _useCaseTuner = useCaseTuner;
            _userContext = userContext;
        }

        PrintRegionalOrdersResponse IPrintRegionalOrderService.PrintRegionalOrder(long orderId)
        {
            var selector = _finder.Find<Order>(x => x.Id == orderId);
            var startDate = _finder.Find<Order>(x => x.Id == orderId).Select(x => x.BeginDistributionDate).Single();

            return PrintRegionalOrder(selector, startDate, false);
        }

        PrintRegionalOrdersResponse IPrintRegionalOrderService.PrintRegionalOrder(long organizationId, DateTime startDate, DateTime endDate)
        {
            _useCaseTuner.AlterDuration<PrintRegionalOrderService>();

            // юр. лицо ДГН
            const int DoubleGisBranchOfficeOrganizationUnit = 47;

            // операция списания в счет оплаты БЗ
            const int AccountDetailOperationTypeId = 7;

            IQueryable<Order> selector;

            var sourceIsUk = organizationId == _moneyDistributionSettings.UkOrganizationUnitId;
            if (sourceIsUk)
            {
                var allowedOrderStates = new[] { OrderState.Approved, OrderState.OnTermination, OrderState.Archive };

                // выбрали УК
                selector = _finder.Find<Order>(x =>
                                x.IsActive && !x.IsDeleted &&
                                x.BranchOfficeOrganizationUnitId != DoubleGisBranchOfficeOrganizationUnit)
                .Where(x => allowedOrderStates.Contains((OrderState)x.WorkflowStepId))
                .Select(x => new
                {
                    Order = x,
                    Lock = x.Locks.FirstOrDefault(y => !y.IsDeleted && y.PeriodStartDate >= startDate && y.PeriodEndDate <= endDate)
                })
                .Where(x => x.Lock != null)
                .Select(x => new
                {
                    x.Order,
                    x.Lock,

                    SourceType = x.Order.SourceOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales).Select(y => (ContributionTypeEnum?)y.BranchOffice.ContributionTypeId).FirstOrDefault(),
                    DestType = x.Order.DestOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales).Select(y => (ContributionTypeEnum?)y.BranchOffice.ContributionTypeId).FirstOrDefault(),
                })
                .Where(x => x.SourceType == ContributionTypeEnum.Branch && x.DestType == ContributionTypeEnum.Franchisees)
                .Select(x => x.Order);
            }
            else
            {
                // выбрали один город
                selector = _finder.Find<Order>(x =>
                                x.SourceOrganizationUnitId == organizationId &&
                                x.IsActive && !x.IsDeleted &&
                                x.BranchOfficeOrganizationUnitId != DoubleGisBranchOfficeOrganizationUnit)
                .Select(x => new
                {
                    Order = x,

                    Lock = x.Locks
                        .FirstOrDefault(y => !y.IsDeleted &&
                                    y.PeriodStartDate >= startDate && y.PeriodEndDate <= endDate &&
                                    y.DebitAccountDetailId != null &&
                                    y.AccountDetail.OperationTypeId == AccountDetailOperationTypeId),
                })
                .Where(x => x.Lock != null)
                .Select(x => x.Order);                
            }

            // если выбрали период позже 1 апреля 13 года, то у заказов должно быть начало размещения позже 1 апреля
            if (startDate >= _moneyDistributionSettings.FirstApril)
            {
                selector = selector.Where(x => x.BeginDistributionDate >= _moneyDistributionSettings.FirstApril);
            }

            return PrintRegionalOrder(selector, startDate, sourceIsUk);
        }

        private static MemoryStream GetPrintTemplate(ContributionTypeEnum destContributionType)
        {
            string settingName;
            switch (destContributionType)
            {
                // с НДС
                case ContributionTypeEnum.Branch:
                    settingName = "VatEnabledDocumentTemplate";
                    break;

                // без НДС
                case ContributionTypeEnum.Franchisees:
                    settingName = "VatDisabledDocumentTemplate";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("destContributionType");
            }

            var memoryStream = new MemoryStream();

            var relativeFilePath = ConfigurationManager.AppSettings[settingName];
            var absoluteFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeFilePath);
            using (var fileStream = new FileStream(absoluteFilePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(memoryStream);
            }

            return memoryStream;
        }

        private static string FormatNumber(string sourceSyncCode1C, string destSyncCode1C, DateTime startDate)
        {
            return string.Format("{0}-{1}-{2}-{3}", sourceSyncCode1C, destSyncCode1C, startDate.ToString("MM"), startDate.ToString("yy"));
        }

        private static DateTime GetOfferDate(DateTime dateTime)
        {
            // offerDate should not be day off
            var offerDate = dateTime.AddMonths(-1).AddDays(22);
            if (offerDate.DayOfWeek == DayOfWeek.Saturday)
            {
                offerDate = offerDate.AddDays(-1);
            }
            else if (offerDate.DayOfWeek == DayOfWeek.Sunday)
            {
                offerDate = offerDate.AddDays(-2);
            }

            return offerDate;
        }

        private PrintRegionalOrdersResponse PrintRegionalOrder(IQueryable<Order> selector, DateTime startDate, bool sourceIsUk)
        {
            var orderInfos = selector.Select(x => new
            {
                x.Id,

                x.FirmId,
                x.BeginDistributionDate,
                SourceBou = x.SourceOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales).Select(y => new BouInfo
                {
                    Id = y.Id,

                    // ReSharper disable once PossibleInvalidOperationException
                    ContributionType = (ContributionTypeEnum)y.BranchOffice.ContributionTypeId.Value,
                }).FirstOrDefault(),

                DestBou = x.DestOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales).Select(y => new BouInfo
                {
                    Id = y.Id,

                    // ReSharper disable once PossibleInvalidOperationException
                    ContributionType = (ContributionTypeEnum)y.BranchOffice.ContributionTypeId.Value,
                }).FirstOrDefault(),

                OrderPositions = x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted && y.PricePosition.Cost > 0).Select(y => new
                {
                    y.Id,
                    y.PricePosition.PositionId,
                    y.PricePosition.PriceId,

                    y.PricePosition.Position.IsComposite,
                    y.CategoryRate,
                    PricePositionCost = y.PricePosition.Cost,

                    y.Amount,
                    y.CalculateDiscountViaPercent,
                    DiscountSum = y.DiscountSum / y.Order.ReleaseCountPlan,
                    y.DiscountPercent,

                    Platform = (PlatformEnum)y.PricePosition.Position.Platform.DgppId,
                }),
            })
            .Where(x => x.OrderPositions.Any())
            .ToArray();

            var printDataDtos = orderInfos.SelectMany(x =>
            {
                if (x.SourceBou == null || x.DestBou == null)
                {
                    throw new ApplicationException("Не найдено основное юр.лицо для региональных продаж");
                }

                var calculator = new MoneyDistributionCalculator(x.SourceBou.ContributionType, x.DestBou.ContributionType, x.SourceBou.Id == x.DestBou.Id, x.BeginDistributionDate < _moneyDistributionSettings.FirstApril);

                return x.OrderPositions.SelectMany(y =>
                {
                    var orderPositionDto = new OrderPositionDto
                    {
                        PositionId = y.PositionId,
                        PriceId = y.PriceId,

                        CategoryRate = y.CategoryRate,

                        Amount = y.Amount,
                        CalculateDiscountViaPercent = y.CalculateDiscountViaPercent,
                        DiscountSum = y.DiscountSum,
                        DiscountPercent = y.DiscountPercent,
                        PricePositionCost = y.PricePositionCost,

                        ExtendedPlatform = _moneyDistributionSettings.ExtendedPlatformMap[y.Platform],
                    };

                    var platformCosts = calculator.CalculatePlatformCosts(orderPositionDto).ToArray();

                    // пакеты нужно схлопнуть обратно, если распределения для всех платформ идут в одну и ту же точку продаж (сейчас это случай франч-филиал)
                    if (y.IsComposite)
                    {
                        var platformCostsGroups = platformCosts
                            .GroupBy(z => new { z.From, z.To, z.NewTo, DefinedInSalesSchema = z.DiscountCost != decimal.Zero })
                            .Select(z => new PlatformCost
                        {
                            PositionId = y.PositionId,
                            From = z.Key.From,
                            To = z.Key.To,
                            NewTo = z.Key.NewTo,
                            Platform = _moneyDistributionSettings.ExtendedPlatformMap[y.Platform],
                            Cost = z.Sum(p => p.Cost),
                            DiscountCost = z.Sum(p => p.DiscountCost),
                            PayablePlan = z.Sum(p => p.PayablePlan),
                            PayablePrice = z.Sum(p => p.PayablePrice),
                        }).ToArray();

                        if (platformCostsGroups.Length == 1)
                        {
                            platformCosts = platformCostsGroups;
                        }
                    }

                    return platformCosts.Where(w => w.DiscountCost != decimal.Zero).Select(z => new PrintDataDto
                        {
                            OrderId = x.Id,
                            OrderPositionId = y.Id,
                            PlatformCost = z,

                            SourceBouId = sourceIsUk ? _moneyDistributionSettings.UkBouId : x.SourceBou.Id,
                            DestBouId = (z.NewTo == ESalesPointType.Uk) ? _moneyDistributionSettings.UkBouId : x.DestBou.Id,
                            ApplicationStringBouId = x.DestBou.Id
                        });
                });
            }).ToArray();

            // TODO: выставляем культуру в тред для _printFormService, удалить после рефакторинга engine печати
            Thread.CurrentThread.CurrentCulture = _userContext.Profile.UserLocaleInfo.UserCultureInfo;

            var responseItems = printDataDtos.GroupBy(x => new { x.DestBouId, x.SourceBouId }).Select(x =>
            {
                var printData = GetPrintData(x, x.Key.SourceBouId, x.Key.DestBouId, startDate);

                using (var memoryStream = GetPrintTemplate(printData.Dest.ContributionType))
                {
                    _printFormService.PrintToDocx(memoryStream, printData, 643);

                    return new PrintRegionalOrdersResponseItem
                    {
                        SourceOrganizationUnitId = printData.Source.OrganizationUnitId,
                        SourceBranchOfficeOrganizationUnitId = printData.Source.Id,

                        DestOrganizationUnitId = printData.Dest.OrganizationUnitId,
                        DestBranchOfficeOrganizationUnitId = printData.Dest.Id,
                        DestOrganizationUnitSyncCode1C = printData.Dest.OrganizationUnitSyncCode1C,

                        TotalAmount = printData.Summary.PayablePlan,

                        FirmWithOrders = printData.Positions.GroupBy(y => y.FirmId).Select(y => new FirmWithOrders
                        {
                            FirmId = y.Key,
                            OrderIds = y.GroupBy(z => z.OrderId).Select(z => z.Key).ToArray(),
                        }).ToArray(),

                        File = new FileDescription
                        {
                            FileName = printData.Number + ".docx",
                            ContentType = MediaTypeNames.Application.Octet,
                            Stream = memoryStream.ToArray(),
                        },
                    };
                }
            });

            return new PrintRegionalOrdersResponse { Items = responseItems.ToArray() };
        }

        private PrintData GetPrintData(IEnumerable<PrintDataDto> printDataDtos, long sourceBouId, long destBouId, DateTime startDate)
        {
            const decimal DiscountPercentConst = 60m;

            var source = GetBouPrintData(sourceBouId);
            var dest = GetBouPrintData(destBouId);
            

            var offerDate = GetOfferDate(startDate);

            var printData = new PrintData
            {
                Number = FormatNumber(source.OrganizationUnitSyncCode1C, dest.OrganizationUnitSyncCode1C, startDate),

                Date = offerDate,
                AdvMatherialsDeadline = offerDate,
                PaymentDate = startDate.AddDaysWithDayOffs(4),

                Dest = dest,
                Source = source,

                Positions = printDataDtos.GroupBy(dto => dto.ApplicationStringBouId)
                .SelectMany(dto =>
                {
                    var appStringBou = GetBouPrintData(dto.Key);

                    return dto.Select(x =>
                        {
                            var positionQuery = _finder.FindAll<Position>();

                            var orderPrintData = _finder.Find<Order>(y => y.Id == x.OrderId).Select(y => new
                            {
                                FirmName = y.Firm.Name,
                                y.DestOrganizationUnit.ElectronicMedia,
                                ReleaseNumber = y.OrderReleaseTotals.Where(z => z.ReleaseBeginDate == startDate).Select(z => (int?)z.ReleaseNumber).FirstOrDefault(),

                                OrderPositionAmount = y.OrderPositions.Where(z => z.Id == x.OrderPositionId).Select(z => z.Amount).FirstOrDefault(),

                                PositionName = positionQuery.Where(z => z.Id == x.PlatformCost.PositionId).Select(z => z.Name).FirstOrDefault(),

                                PositionNameGroups = y.OrderPositions
                                    .Where(z => z.Id == x.OrderPositionId)
                                    .SelectMany(z => z.OrderPositionAdvertisements)
                                    .Where(z => z.PositionId == x.PlatformCost.PositionId)
                                    .GroupBy(z => z.FirmAddress)
                                    .Select(z => new
                                    {
                                        FirmAddressName = (z.Key != null) ? z.Key.Address + ((z.Key.ReferencePoint == null) ? string.Empty : " — " + z.Key.ReferencePoint) : null,
                                        CategoryNames = z.Where(p => p.CategoryId != null).Select(p => p.Category.Name),
                                    }),

                                // system fileds
                                FirmId = y.FirmId,
                                OrderId = y.Id,
                                DestOrganizationUnitName = y.DestOrganizationUnit.Name,
                            }).Single();

                            // applicationString
                            string applicationString;
                            switch (x.PlatformCost.Platform)
                            {
                                case PlatformsExtended.None:
                                    applicationString = string.Format(
                                        "1. Электронное СМИ: «{0}» (версия для ПК и мобильные версии). Свидетельство о регистрации ЭЛ № {1}. Выпуск СМИ № {2}. " + Environment.NewLine +
                                          "2. Интернет-площадка 2gis.ru. " + Environment.NewLine +
                                          "3. Интернет-площадки и Веб-приложения, указанные в Прайс-листе на дату размещения рекламы.",
                                        appStringBou.ElectronicMedia,
                                        appStringBou.RegistrationCertificate,
                                        orderPrintData.ReleaseNumber == 0 ? null : orderPrintData.ReleaseNumber);
                                    break;
                                case PlatformsExtended.Desktop:
                                    applicationString = string.Format(
                                        "Электронное СМИ: «{0}» (версия для ПК). Свидетельство о регистрации ЭЛ № {1}. Выпуск СМИ № {2}.",
                                        appStringBou.ElectronicMedia,
                                        appStringBou.RegistrationCertificate,
                                        orderPrintData.ReleaseNumber == 0 ? null : orderPrintData.ReleaseNumber);
                                    break;
                                case PlatformsExtended.Mobile:
                                    applicationString = string.Format(
                                        "Электронное СМИ: «{0}» (мобильные версии). Свидетельство о регистрации ЭЛ № {1}. Выпуск СМИ № {2}.",
                                        appStringBou.ElectronicMedia,
                                        appStringBou.RegistrationCertificate,
                                        orderPrintData.ReleaseNumber == 0 ? null : orderPrintData.ReleaseNumber);
                                    break;
                                case PlatformsExtended.Api:
                                case PlatformsExtended.ApiOnline:
                                case PlatformsExtended.ApiPartner:
                                    applicationString = string.Format(
                                        "Интернет-площадки и Веб-приложения указанные в Прайс-листе на дату размещения рекламы.");
                                    break;
                                case PlatformsExtended.Online:
                                    applicationString = "Интернет-площадка 2gis.ru.";
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            // positionName
                            var positionName = orderPrintData.PositionName;
                            var positionAdditionalName = (string)null;
                            foreach (var namesGroup in orderPrintData.PositionNameGroups)
                            {
                                var categoryNames = string.Join(", ", namesGroup.CategoryNames);

                                if (!string.IsNullOrEmpty(namesGroup.FirmAddressName))
                                {
                                    positionAdditionalName = namesGroup.FirmAddressName + ": " + categoryNames;
                                }
                                else
                                {
                                    positionAdditionalName = categoryNames;
                                }
                            }

                            if (!string.IsNullOrEmpty(positionAdditionalName))
                            {
                                positionName += ": " + positionAdditionalName;
                            }

                            // payable
                            var payablePrintData = new PayablePrintData();
                            switch (dest.ContributionType)
                            {
                                // с НДС
                                case ContributionTypeEnum.Branch:
                                    {
                                        payablePrintData.PayablePlan = x.PlatformCost.DiscountCost;

                                        var payablePlanWoVatExact = x.PlatformCost.DiscountCost / 1.18m;
                                        payablePrintData.PayablePlanWoVat = Math.Round(payablePlanWoVatExact, 2, MidpointRounding.ToEven);

                                        payablePrintData.PayablePlanVat = payablePrintData.PayablePlan - payablePrintData.PayablePlanWoVat;
                                    }

                                    break;

                                // без НДС
                                case ContributionTypeEnum.Franchisees:
                                    {
                                        payablePrintData.PayablePlan = x.PlatformCost.DiscountCost;
                                        payablePrintData.PayablePlanWoVat = x.PlatformCost.DiscountCost;
                                        payablePrintData.PayablePlanVat = 0m;
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            // pricePerUnit
                            var pricePerUnitExact = x.PlatformCost.Cost / orderPrintData.OrderPositionAmount;
                            var pricePerUnit = Math.Round(pricePerUnitExact, 2, MidpointRounding.ToEven);

                            // pricePerUnitWithDiscount 
                            var pricePerUnitWithDiscountExact = x.PlatformCost.DiscountCost / orderPrintData.OrderPositionAmount;
                            var pricePerUnitWithDiscount = Math.Round(pricePerUnitWithDiscountExact, 2, MidpointRounding.ToEven);

                            return new PositionPrintData
                            {
                                FirmName = orderPrintData.FirmName,
                                StartDate = startDate,
                                ApplicationString = applicationString,
                                PositionName = positionName,

                                DiscountPercent = DiscountPercentConst,

                                Payable = payablePrintData,
                                Amount = orderPrintData.OrderPositionAmount,

                                PricePerUnit = pricePerUnit,
                                PricePerUnitWithDiscount = pricePerUnitWithDiscount,

                                // system fields
                                FirmId = orderPrintData.FirmId,
                                OrderId = orderPrintData.OrderId,
                                DestOrganizationUnitName = orderPrintData.DestOrganizationUnitName,
                            };
                        });
                })
                .OrderBy(x => x.DestOrganizationUnitName) // сортировка позиций
                .ThenBy(x => x.FirmName)
                .ToArray(),
            };

            // summary
            printData.Summary = new PayablePrintData
            {
                PayablePlan = printData.Positions.Sum(y => y.Payable.PayablePlan),
                PayablePlanWoVat = printData.Positions.Sum(y => y.Payable.PayablePlanWoVat),
                PayablePlanVat = printData.Positions.Sum(y => y.Payable.PayablePlanVat),
            };

            return printData;
        }

        private BouPrintData GetBouPrintData(long bouId)
        {
            var bouPrintData = _finder.Find<BranchOfficeOrganizationUnit>(y => y.Id == bouId).Select(y => new BouPrintData
            {
                OrganizationUnitId = y.OrganizationUnitId,
                OrganizationUnitSyncCode1C = y.OrganizationUnit.SyncCode1C,
                BranchOfficeInn = y.BranchOffice.Inn,
                ContributionType = (ContributionTypeEnum)y.BranchOffice.ContributionTypeId.Value,
                BranchOfficeLegalAddress = y.BranchOffice.LegalAddress,

                Id = y.Id,
                ShortLegalName = y.ShortLegalName,
                PositionInGenitive = y.PositionInGenitive,
                ChiefNameInGenitive = y.ChiefNameInGenitive,
                ChiefNameInNominative = y.ChiefNameInNominative,
                OperatesOnTheBasisInGenitive = y.OperatesOnTheBasisInGenitive,
                Email = y.Email,
                Kpp = y.Kpp,
                PaymentEssentialElements = y.PaymentEssentialElements,

                ElectronicMedia = y.OrganizationUnit.ElectronicMedia,
                RegistrationCertificate = y.RegistrationCertificate,
            }).Single();

            // коммерция привыкла к "01", печатаем его а не то что в базе
            if (bouId == _moneyDistributionSettings.UkBouId)
            {
                bouPrintData.OrganizationUnitSyncCode1C = "01";
            }

            return bouPrintData;
        }

        # region Nested types

        private sealed class PrintDataDto
        {
            public PlatformCost PlatformCost { get; set; }
            public long OrderId { get; set; }
            public long OrderPositionId { get; set; }
            public long SourceBouId { get; set; }
            public long DestBouId { get; set; }
            public long ApplicationStringBouId { get; set; }
        }

        private sealed class BouInfo
        {
            public long Id { get; set; }
            public ContributionTypeEnum ContributionType { get; set; }
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private sealed class PrintData
        {
            public string Number { get; set; }
            public DateTime Date { get; set; }
            public DateTime AdvMatherialsDeadline { get; set; }
            public DateTime PaymentDate { get; set; }
            public BouPrintData Source { get; set; }
            public BouPrintData Dest { get; set; }
            public PositionPrintData[] Positions { get; set; }
            public PayablePrintData Summary { get; set; }
        }
        private sealed class PositionPrintData
        {
            public string FirmName { get; set; }
            public DateTime StartDate { get; set; }
            public string ApplicationString { get; set; }
            public string PositionName { get; set; }
            public decimal DiscountPercent { get; set; }
            public PayablePrintData Payable { get; set; }
            public int Amount { get; set; }
            public decimal PricePerUnit { get; set; }
            public decimal PricePerUnitWithDiscount { get; set; }

            // system fileds
            public long FirmId { get; set; }
            public long OrderId { get; set; }
            public string DestOrganizationUnitName { get; set; }
        }

        private sealed class PayablePrintData
        {
            public decimal PayablePlan { get; set; }
            public decimal PayablePlanWoVat { get; set; }
            public decimal PayablePlanVat { get; set; }
        }

        private sealed class BouPrintData
        {
            public long OrganizationUnitId { get; set; }
            public string OrganizationUnitSyncCode1C { get; set; }
            public string BranchOfficeInn { get; set; }
            public string BranchOfficeLegalAddress { get; set; }

            public long Id { get; set; }
            public string ShortLegalName { get; set; }
            public string PositionInGenitive { get; set; }
            public string ChiefNameInGenitive { get; set; }
            public string ChiefNameInNominative { get; set; }
            public string OperatesOnTheBasisInGenitive { get; set; }
            public string Email { get; set; }
            public string Kpp { get; set; }
            public string PaymentEssentialElements { get; set; }
            public string ElectronicMedia { get; set; }
            public string RegistrationCertificate { get; set; }

            // system fields
            public ContributionTypeEnum ContributionType { get; set; }
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local

        # endregion
    }
}