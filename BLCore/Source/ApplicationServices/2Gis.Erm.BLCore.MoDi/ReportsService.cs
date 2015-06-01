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
using DoubleGis.Erm.BLCore.API.MoDi.Reports;
using DoubleGis.Erm.BLCore.API.MoDi.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Security.API.UserContext;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.BLCore.MoDi
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class ReportsService : IReportsService
    {
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IFinder _finder;
        private readonly IMoneyDistributionSettings _moneyDistributionSettings;
        private readonly IUserContext _userContext;
        private readonly IPrintFormService _printFormService;

        public ReportsService(IFinder finder, IUseCaseTuner useCaseTuner, IMoneyDistributionSettings moneyDistributionSettings, IUserContext userContext, IPrintFormService printFormService)
        {
            _finder = finder;
            _useCaseTuner = useCaseTuner;
            _moneyDistributionSettings = moneyDistributionSettings;
            _userContext = userContext;
            _printFormService = printFormService;
        }

        public PlatformReportResponse PlatformReport(PlatformReportRequest request)
        {
            _useCaseTuner.AlterDuration<ReportsService>();

            var lessStartDateStates = new[] { OrderState.Approved, OrderState.Archive, OrderState.OnTermination };

            var equalsStartDateStates = (IEnumerable<OrderState>)new[] { OrderState.Approved, OrderState.Archive, OrderState.OnTermination };
            if (request.SelectOrdersOnApproval)
            {
                equalsStartDateStates = equalsStartDateStates.Concat(new[] { OrderState.OnApproval });
            }
            if (request.SelectOrdersOnRegistration)
            {
                equalsStartDateStates = equalsStartDateStates.Concat(new[] { OrderState.OnRegistration });
            }

            var orderInfos = _finder.FindObsolete(new FindSpecification<Order>(
                        x => (x.SourceOrganizationUnitId == request.OrganizationUnitId
                                || x.DestOrganizationUnitId == request.OrganizationUnitId
                                || request.GetDataForAllNetwork) &&
                            x.EndDistributionDateFact > request.StartDate &&
                            x.IsActive && !x.IsDeleted))
                //.Where(x => x.Id == 260807) // uncomment for test
            .Select(x => new
            {
                Order = x,
                LessStartDate = x.BeginDistributionDate < request.StartDate,
                EqualsStartDate = x.BeginDistributionDate == request.StartDate,
            })
            .Where(x => x.LessStartDate && lessStartDateStates.Contains(x.Order.WorkflowStepId) ||
                        x.EqualsStartDate && equalsStartDateStates.Contains(x.Order.WorkflowStepId))
            .Select(x => x.Order)
            .Select(x => new
            {
                FirmName = x.Firm.Name,
                DestOrganizationUnitName = x.DestOrganizationUnit.Name,
                SourceOrganizationUnitName = x.SourceOrganizationUnit.Name,
                OrderType = x.OrderType,
                x.DiscountPercent,
                ClientName = x.Deal.Client.Name,
                x.OwnerCode,
                LegalPersonName = x.LegalPerson.ShortName,
                BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnit.ShortLegalName,
                OrderNumber = x.Number,
                x.CreatedOn,
                x.BeginDistributionDate,
                x.EndDistributionDatePlan,
                x.ReleaseCountPlan,
                OrderState = x.WorkflowStepId,
                x.EndDistributionDateFact,
                x.ReleaseCountFact,

                // system properties
                x.Id,
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
                    y.PricePosition.PositionId,
                    y.PricePosition.PriceId,

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

            var items = orderInfos.SelectMany(x =>
            {
                if (x.SourceBou == null || x.DestBou == null)
                {
                    throw new ApplicationException("Не найдено основное юр.лицо для региональных продаж");
                }

                var calculator = new MoneyDistributionCalculator(x.SourceBou.ContributionType, x.DestBou.ContributionType, x.SourceBou.Id == x.DestBou.Id, x.BeginDistributionDate < _moneyDistributionSettings.FirstApril);

                var ownerName = _finder.FindObsolete(new FindSpecification<User>(y => y.Id == x.OwnerCode)).Select(y => y.DisplayName).Single();

                var ratedOrderPosition = x.OrderPositions.FirstOrDefault(y => y.CategoryRate != 1m);
                var reportRate = ratedOrderPosition != null ? (decimal?)ratedOrderPosition.CategoryRate : null;

                var platformCosts = x.OrderPositions.SelectMany(y =>
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

                    return calculator.CalculatePlatformCosts(orderPositionDto);
                });

                return platformCosts.Select(z =>
                {
                    // в отчёте без разделения Api считаем все виды Api за один
                    if (request.Type == (int)PlatformReportType.DoNotSplitApi &&
                        (z.Platform == PlatformsExtended.ApiOnline || z.Platform == PlatformsExtended.ApiPartner))
                    {
                        z.Platform = PlatformsExtended.Api;
                    }

                    return z;
                })
                .GroupBy(z => z.Platform)
                .Select(z => new PlatformCost
                {
                    Platform = z.Key,
                    PayablePlan = z.Sum(p => p.PayablePlan),
                    PayablePrice = z.Sum(p => p.PayablePrice)
                })
                .Select(z => new PlatformReportItem
                {
                    FirmName = x.FirmName,
                    SourceOrganizationUnitName = x.SourceOrganizationUnitName,
                    DestOrganizationUnitName = x.DestOrganizationUnitName,
                    OrderType = x.OrderType.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    DiscountPercent = x.DiscountPercent,
                    ClientName = x.ClientName,
                    OwnerName = ownerName,
                    LegalPersonName = x.LegalPersonName,
                    BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnitName,
                    OrderNumber = x.OrderNumber,
                    CreatedOn = x.CreatedOn,
                    BeginDistributionDate = x.BeginDistributionDate,
                    EndDistributionDatePlan = x.EndDistributionDatePlan,
                    ReleaseCountPlan = x.ReleaseCountPlan,
                    OrderState = x.OrderState.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    EndDistributionDateFact = x.EndDistributionDateFact,
                    ReleaseCountFact = x.ReleaseCountFact,
                    Rate = reportRate,

                    PlatformName = z.Platform.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    PayablePlan = z.PayablePlan,
                    PayablePrice = z.PayablePrice,
                });
            });

            return new PlatformReportResponse { PlatformReportItems = items.ToArray() };
        }

        public MoneyDistributionReportResponse MoneyDistributionReport(MoneyDistributionReportRequest request)
        {
            _useCaseTuner.AlterDuration<ReportsService>();

            var lessStartDateStates = new[] { OrderState.Approved, OrderState.Archive, OrderState.OnTermination };
            var equalsStartDateStates = (IEnumerable<OrderState>)new[] { OrderState.Approved, OrderState.Archive, OrderState.OnTermination };
            if (request.SelectOrdersOnApproval)
            {
                equalsStartDateStates = equalsStartDateStates.Concat(new[] { OrderState.OnApproval });
            }
            if (request.SelectOrdersOnRegistration)
            {
                equalsStartDateStates = equalsStartDateStates.Concat(new[] { OrderState.OnRegistration });
            }

            var firstApril = _moneyDistributionSettings.FirstApril;

            var orderInfos = _finder.FindObsolete(new FindSpecification<Order>(
                x =>
                    (x.SourceOrganizationUnitId == request.OrganizationUnitId
                    || x.DestOrganizationUnitId == request.OrganizationUnitId
                    || request.GetDataForAllNetwork) &&
                        x.EndDistributionDateFact > request.StartDate &&
                        x.IsActive && !x.IsDeleted))
            .Select(x => new
            {
                Order = x,
                LessStartDate = x.BeginDistributionDate < request.StartDate,
                EqualsStartDate = x.BeginDistributionDate == request.StartDate,
            })
            .Where(x => x.LessStartDate && lessStartDateStates.Contains(x.Order.WorkflowStepId) ||
                        x.EqualsStartDate && equalsStartDateStates.Contains(x.Order.WorkflowStepId))
            .Select(x => x.Order)
            .Select(x => new
            {
                FirmName = x.Firm.Name,
                DestOrganizationUnitName = x.DestOrganizationUnit.Name,
                SourceOrganizationUnitName = x.SourceOrganizationUnit.Name,
                OrderType = x.OrderType,
                x.DiscountPercent,
                ClientName = x.Deal.Client.Name,
                x.OwnerCode,
                LegalPersonName = x.LegalPerson.ShortName,
                BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnit.ShortLegalName,
                OrderNumber = x.Number,
                x.CreatedOn,
                x.BeginDistributionDate,
                x.EndDistributionDatePlan,
                x.ReleaseCountPlan,
                OrderState = x.WorkflowStepId,
                x.EndDistributionDateFact,
                x.ReleaseCountFact,
                Platform = (PlatformEnum)x.Platform.DgppId,

                // system properties
                x.Id,
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
                    y.PricePosition.PositionId,
                    y.PricePosition.PriceId,

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
                // отфильтровываем франч-франч по API\Online у которых период размещения до 1 апреля 2013
            .Where(x => !(x.SourceBou.ContributionType == ContributionTypeEnum.Franchisees && x.DestBou.ContributionType == ContributionTypeEnum.Franchisees &&
                        x.BeginDistributionDate < firstApril &&
                        (x.Platform == PlatformEnum.Api || x.Platform == PlatformEnum.Online)))
            .ToArray();

            var items = orderInfos.SelectMany(x =>
            {
                if (x.SourceBou == null || x.DestBou == null)
                {
                    throw new ApplicationException("Не найдено основное юр.лицо для региональных продаж");
                }

                var calculator = new MoneyDistributionCalculator(x.SourceBou.ContributionType, x.DestBou.ContributionType, x.SourceBou.Id == x.DestBou.Id, x.BeginDistributionDate < _moneyDistributionSettings.FirstApril);

                var ownerName = _finder.FindObsolete(new FindSpecification<User>(y => y.Id == x.OwnerCode)).Select(y => y.DisplayName).Single();

                var ratedOrderPosition = x.OrderPositions.FirstOrDefault(y => y.CategoryRate != 1m);
                var reportRate = ratedOrderPosition != null ? (decimal?)ratedOrderPosition.CategoryRate : null;

                var platformCosts = x.OrderPositions.SelectMany(y =>
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

                    return calculator.CalculatePlatformCosts(orderPositionDto);
                });

                return platformCosts.Select(z =>
                {
                    // всегда группируем по Api
                    if (z.Platform == PlatformsExtended.ApiOnline || z.Platform == PlatformsExtended.ApiPartner)
                    {
                        z.Platform = PlatformsExtended.Api;
                    }

                    return z;
                })
                .GroupBy(z => new { z.Platform, z.From, z.To, z.NewTo })
                .Select(z => new PlatformCost
                {
                    Platform = z.Key.Platform,
                    From = z.Key.From,
                    To = z.Key.To,
                    NewTo = z.Key.NewTo,

                    PayablePlan = z.Sum(p => p.PayablePlan),
                    PayablePrice = z.Sum(p => p.PayablePrice),
                    Cost = z.Sum(p => p.Cost),
                    DiscountCost = z.Sum(p => p.DiscountCost),
                })
                .Select(z => new MoneyDistributionReportItem
                {
                    FirmName = x.FirmName,
                    SourceOrganizationUnitName = x.SourceOrganizationUnitName,
                    DestOrganizationUnitName = x.DestOrganizationUnitName,
                    OrderType = x.OrderType.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    DiscountPercent = x.DiscountPercent,
                    ClientName = x.ClientName,
                    OwnerName = ownerName,
                    LegalPersonName = x.LegalPersonName,
                    BranchOfficeOrganizationUnitName = x.BranchOfficeOrganizationUnitName,
                    OrderNumber = x.OrderNumber,
                    CreatedOn = x.CreatedOn,
                    BeginDistributionDate = x.BeginDistributionDate,
                    EndDistributionDatePlan = x.EndDistributionDatePlan,
                    ReleaseCountPlan = x.ReleaseCountPlan,
                    OrderState = x.OrderState.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    EndDistributionDateFact = x.EndDistributionDateFact,
                    ReleaseCountFact = x.ReleaseCountFact,
                    Rate = reportRate,

                    PlatformName = z.Platform.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    PayablePlan = z.PayablePlan,
                    PayablePrice = z.PayablePrice,

                    // 4 колонки отличают этот отчёт от предыдущего
                    From = z.From.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    To = z.To.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    SaleType = GetSalesPointTypePair(z.From, z.To, x.SourceBou.Id == x.DestBou.Id).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    DiscountCost = z.DiscountCost,
                });
            });

            return new MoneyDistributionReportResponse { MoneyDistributionReportItems = items.ToArray() };
        }

        private static ESalesPointTypePair GetSalesPointTypePair(ESalesPointType from, ESalesPointType to, bool isSelf)
        {
            var realTo = isSelf ? ESalesPointType.Self : to;

            var nonParsed = from.ToString() + realTo;
            return (ESalesPointTypePair)Enum.Parse(typeof(ESalesPointTypePair), nonParsed);
        }

        public FileDescription ProceedsReport(DateTime startDate, DateTime endDate)
        {
            var template = ConfigurationManager.AppSettings["ProceedsReportTemplate"];
            using (var memoryStream = new MemoryStream())
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, template);
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(memoryStream);
                }

                var reportData = GetProceedsReportData(startDate, endDate);

                // TODO: выставляем культуру в тред для _printFormService, удалить после рефакторинга engine печати
                Thread.CurrentThread.CurrentCulture = _userContext.Profile.UserLocaleInfo.UserCultureInfo;
                _printFormService.PrintToDocx(memoryStream, reportData, 643);

                var fileDescription = new FileDescription
                {
                    FileName = string.Format("Отчет по выручке-{0}.docx", startDate.ToString("MM", _userContext.Profile.UserLocaleInfo.UserCultureInfo)),
                    ContentType = MediaTypeNames.Application.Octet,
                    Stream = memoryStream.ToArray(),
                };

                return fileDescription;
            }
        }

        private object GetProceedsReportData(DateTime startDate, DateTime endDate)
        {
            _useCaseTuner.AlterDuration<ReportsService>();
            var allowedOrderStates = new[] { OrderState.Approved, OrderState.Archive, OrderState.OnTermination };

            // for testing purposes
            //var allowedOrgUnits = new long[] { 6 };

            var orderInfos =

                _finder.FindObsolete(new FindSpecification<Order>(x => x.IsActive && !x.IsDeleted))
                //.Where(x => allowedOrgUnits.Contains(x.DestOrganizationUnitId) || allowedOrgUnits.Contains(x.SourceOrganizationUnitId)) // uncomment to test
                    .Where(x => allowedOrderStates.Contains(x.WorkflowStepId) && x.Locks.Any(y => !y.IsDeleted && y.PeriodStartDate == startDate && y.PeriodEndDate == endDate))
                    .Select(x => new
                    {
                        Order = x,
                        OrderPositions = x.OrderPositions.Where(z => z.IsActive && !z.IsDeleted && z.PricePosition.Cost > 0)
                            .Select(op => new
                            {
                                op.PricePosition.PositionId,
                                op.PricePosition.PriceId,
                                Platform = (PlatformEnum)op.PricePosition.Position.Platform.DgppId,
                                op.Amount,
                                op.CategoryRate,
                                op.CalculateDiscountViaPercent,
                                DiscountSum = op.DiscountSum / op.Order.ReleaseCountPlan,
                                op.DiscountPercent,
                                PricePositionCost = op.PricePosition.Cost
                            }),
                        SourceBou = x.SourceOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales)
                            .Select(y => new
                            {
                                y.Id,
                                y.OrganizationUnitId,
                                // ReSharper disable once PossibleInvalidOperationException
                                BranchOfficeContributionType = (ContributionTypeEnum)y.BranchOffice.ContributionTypeId
                            }).FirstOrDefault(),
                        DestBou = x.DestOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales)
                            .Select(y => new
                            {
                                y.Id,
                                y.OrganizationUnitId,
                                // ReSharper disable once PossibleInvalidOperationException
                                BranchOfficeContributionType = (ContributionTypeEnum)y.BranchOffice.ContributionTypeId
                            }).FirstOrDefault()
                    }).ToArray();


            var distributions = orderInfos
                .SelectMany(x =>
                {
                    var orderPositionDtos = x.OrderPositions.Select(orderPosition => new OrderPositionDto
                    {
                        PositionId = orderPosition.PositionId,
                        PriceId = orderPosition.PriceId,
                        ExtendedPlatform = _moneyDistributionSettings.ExtendedPlatformMap[orderPosition.Platform],
                        Amount = orderPosition.Amount,
                        CategoryRate = orderPosition.CategoryRate,
                        CalculateDiscountViaPercent = orderPosition.CalculateDiscountViaPercent,
                        DiscountSum = orderPosition.DiscountSum,
                        DiscountPercent = orderPosition.DiscountPercent,
                        PricePositionCost = orderPosition.PricePositionCost
                    });

                    var moneyDistributionCalculator = new MoneyDistributionCalculator(x.SourceBou.BranchOfficeContributionType, x.DestBou.BranchOfficeContributionType, x.SourceBou.Id == x.DestBou.Id, x.Order.BeginDistributionDate < _moneyDistributionSettings.FirstApril);
                    var temp = orderPositionDtos.SelectMany(moneyDistributionCalculator.CalculatePlatformCosts);

                    return temp.Select(y => new
                    {
                        Distribution = y,
                        x.DestBou,
                        IsSelf = x.DestBou.Id == x.SourceBou.Id
                    });
                });

            var saleDirectionGroup = distributions.GroupBy(x => new { x.Distribution.NewTo, x.IsSelf }).Select(x => new
            {
                PlatformGroup = x.GroupBy(y => y.Distribution.Platform).Select(y => new
                {
                    OrganizationUnitGroup = y.GroupBy(z => z.DestBou.OrganizationUnitId).Select(z => new
                    {
                        x.Key.NewTo,
                        x.Key.IsSelf,
                        PlatformId = y.Key,
                        OrganizationUnitId = z.Key,

                        PayablePlan = z.Sum(p =>
                        {
                            switch (p.Distribution.From)
                            {
                                case ESalesPointType.Filial:
                                    {
                                        switch (p.Distribution.NewTo)
                                        {
                                            // из филиала в филиал учитываем 100 %
                                            case ESalesPointType.Filial:
                                                {
                                                    return p.Distribution.PayablePlan;
                                                }

                                            case ESalesPointType.Franch:
                                                {
                                                    return p.Distribution.DiscountCost;
                                                }

                                            default:
                                                {
                                                    throw new ArgumentOutOfRangeException();
                                                }
                                        }
                                    }

                                case ESalesPointType.Franch:
                                    {
                                        switch (p.Distribution.NewTo)
                                        {
                                            case ESalesPointType.Franch:
                                                {
                                                    // из франча во франч учитываем только то что пошло в УК
                                                    if (y.Key == PlatformsExtended.Desktop || y.Key == PlatformsExtended.Mobile)
                                                    {
                                                        return 0;
                                                    }
                                                }
                                                break;
                                        }

                                        return p.Distribution.DiscountCost;
                                    }

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }),
                    }),
                }),
            }).ToArray();

            var proceedsQuery = saleDirectionGroup.SelectMany(x => x.PlatformGroup.SelectMany(y => y.OrganizationUnitGroup));

            var bouInfos = _finder.FindObsolete(new FindSpecification<BranchOfficeOrganizationUnit>(x => x.IsActive && !x.IsDeleted && x.IsPrimaryForRegionalSales)).Select(x => new
            {
                x.OrganizationUnitId,
                OrganizationUnitName = x.OrganizationUnit.Name,
                x.RegistrationCertificate,
                x.OrganizationUnit.FirstEmitDate
            }).ToArray();

            var proceeds = bouInfos.Join(proceedsQuery, x => x.OrganizationUnitId, x => x.OrganizationUnitId, (x, y) =>
            {
                var payablePlanWoVat = y.PayablePlan / 1.18m;
                var vat = y.PayablePlan - payablePlanWoVat;

                return new
                {
                    // keys
                    y.NewTo,
                    y.IsSelf,
                    y.PlatformId,

                    PlatformName = y.PlatformId.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture),
                    x.OrganizationUnitName,
                    x.RegistrationCertificate,
                    ReleaseNumber = startDate.MonthDifference(x.FirstEmitDate),

                    y.PayablePlan,
                    PayablePlanWoVat = payablePlanWoVat,
                    Vat = vat,
                };
            })
                .OrderBy(x => x.PlatformId)
                .ThenByDescending(x => x.PayablePlan)
                .ToArray();

            var filials =
                proceeds.Where(x => x.NewTo == ESalesPointType.Filial && !x.IsSelf
                    && (x.PlatformId == PlatformsExtended.Desktop || x.PlatformId == PlatformsExtended.Mobile))
                    .ToArray();
            var filialsTotalPayablePlan = filials.Sum(x => x.PayablePlan);
            var filialsTotalPayablePlanWoVat = filialsTotalPayablePlan / 1.18m;
            var filialsTotalVat = filialsTotalPayablePlan - filialsTotalPayablePlanWoVat;

            var franches =
                proceeds.Where(x => x.NewTo == ESalesPointType.Franch && !x.IsSelf && (x.PlatformId == PlatformsExtended.Desktop || x.PlatformId == PlatformsExtended.Mobile))
                    .ToArray();
            var franchesTotalPayablePlan = franches.Sum(x => x.PayablePlan);
            var franchesTotalPayablePlanWoVat = franchesTotalPayablePlan / 1.18m;
            var franchesTotalVat = franchesTotalPayablePlan - franchesTotalPayablePlanWoVat;

            var apiPartners = proceeds.Where(x => x.PlatformId == PlatformsExtended.ApiPartner).ToArray();
            var apiPartnerPayablePlan = apiPartners.Sum(x => x.PayablePlan);
            var apiPartnerPayablePlanWoVat = apiPartnerPayablePlan / 1.18m;
            var apiPartnerVat = apiPartnerPayablePlan - apiPartnerPayablePlanWoVat;

            var apiOnlines = proceeds.Where(x => x.PlatformId == PlatformsExtended.ApiOnline).ToArray();
            var apiOnlinePayablePlan = apiOnlines.Sum(x => x.PayablePlan);
            var apiOnlinePayablePlanWoVat = apiOnlinePayablePlan / 1.18m;
            var apiOnlineVat = apiOnlinePayablePlan - apiOnlinePayablePlanWoVat;

            var onlines = proceeds.Where(x => x.PlatformId == PlatformsExtended.Online).ToArray();
            var onlinePayablePlan = onlines.Sum(x => x.PayablePlan);
            var onlinePayablePlanWoVat = onlinePayablePlan / 1.18m;
            var onlineVat = onlinePayablePlan - onlinePayablePlanWoVat;

            var reportData = new
            {
                MonthNumber = startDate.ToString("MM", _userContext.Profile.UserLocaleInfo.UserCultureInfo),

                NowDate = DateTime.UtcNow,
                StartDate = startDate.ToString("MMMM yyyy", _userContext.Profile.UserLocaleInfo.UserCultureInfo),

                Filials = filials,
                FilialTotals = new
                {
                    PayablePlan = filialsTotalPayablePlan,
                    PayablePlanWoVat = filialsTotalPayablePlanWoVat,
                    Vat = filialsTotalVat,
                },

                Franches = franches,
                FranchTotals = new
                {
                    PayablePlan = franchesTotalPayablePlan,
                    PayablePlanWoVat = franchesTotalPayablePlanWoVat,
                    Vat = franchesTotalVat,
                },

                ApiPartner = new
                {
                    PayablePlan = apiPartnerPayablePlan,
                    PayablePlanWoVat = apiPartnerPayablePlanWoVat,
                    Vat = apiPartnerVat,
                },

                ApiOnline = new
                {
                    PayablePlan = apiOnlinePayablePlan,
                    PayablePlanWoVat = apiOnlinePayablePlanWoVat,
                    Vat = apiOnlineVat,
                },

                Online = new
                {
                    PayablePlan = onlinePayablePlan,
                    PayablePlanWoVat = onlinePayablePlanWoVat,
                    Vat = onlineVat,
                },

                PayablePlan = filialsTotalPayablePlan + franchesTotalPayablePlan + apiPartnerPayablePlan + apiOnlinePayablePlan + onlinePayablePlan,
                Vat = filialsTotalVat + franchesTotalVat + apiPartnerVat + apiOnlineVat + onlineVat,
            };

            return reportData;
        }

        # region Nested types

        // объединить с BouInfo в PrintRegionalOrderService
        private sealed class BouInfo
        {
            public long Id { get; set; }
            public ContributionTypeEnum ContributionType { get; set; }
        }

        # endregion
    }
}