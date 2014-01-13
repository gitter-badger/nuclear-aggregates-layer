using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;

using DoubleGis.Erm.BLCore.API.MoDi.Accounting;
using DoubleGis.Erm.BLCore.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.MoDi;
using DoubleGis.Erm.BLCore.API.MoDi.Dto;
using DoubleGis.Erm.BLCore.API.MoDi.Enums;
using DoubleGis.Erm.BLCore.API.MoDi.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using SaveOptions = System.Xml.Linq.SaveOptions;

namespace DoubleGis.Erm.BLCore.MoDi
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class AccountingSystemService : IAccountingSystemService
    {
        private const string RussianOf = "оф";
        private const string EnglishOf = "oф";

        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly IMoneyDistributionSettings _moneyDistributionSettings;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IBranchOfficeRepository _branchOfficeRepository;
        private readonly IEnumerable<PlatformsExtended> _extendedPlatformList = Enum.GetValues(typeof(PlatformsExtended)).Cast<PlatformsExtended>();

        public AccountingSystemService(IUseCaseTuner useCaseTuner,
                                       IFinder finder,
                                       IUserContext userContext,
                                       IMoneyDistributionSettings moneyDistributionSettings,
                                       ISecurityServiceUserIdentifier userIdentifierService,
                                       IBranchOfficeRepository branchOfficeRepository)
        {
            _useCaseTuner = useCaseTuner;
            _finder = finder;
            _userContext = userContext;
            _moneyDistributionSettings = moneyDistributionSettings;
            _userIdentifierService = userIdentifierService;
            _branchOfficeRepository = branchOfficeRepository;
        }

        public ExportAccountDetailsTo1CResponse ExportAccountDetailsTo1C(long organizationId, DateTime startDate, DateTime endDate)
        {
            Thread.CurrentThread.CurrentCulture = _userContext.Profile.UserLocaleInfo.UserCultureInfo;

            _useCaseTuner.AlterDuration<AccountingSystemService>();

            var requestContributionType = _finder.Find<BranchOfficeOrganizationUnit>(x => x.OrganizationUnitId == organizationId && x.IsActive && !x.IsDeleted && x.IsPrimaryForRegionalSales)
                                                 .Select(x => (ContributionTypeEnum)x.BranchOffice.ContributionTypeId)
                                                 .Single();

            var orderInfosQuery = _finder.Find<Order>(x => !x.IsDeleted);

            // кастомизуем запрос в зависимости от формы сотрудничества
            if (requestContributionType == ContributionTypeEnum.Branch)
            {
                orderInfosQuery = orderInfosQuery.Where(x => x.DestOrganizationUnitId == organizationId);

                // если период позже 1 апреля 13 года, то у заказов должно быть начало размещения позже 1 апреля
                if (startDate >= _moneyDistributionSettings.FirstApril)
                {
                    orderInfosQuery = orderInfosQuery.Where(x => x.BeginDistributionDate >= _moneyDistributionSettings.FirstApril);
                }
            }
            else if (requestContributionType == ContributionTypeEnum.Franchisees)
            {
                orderInfosQuery = orderInfosQuery.Where(x =>

                    !(x.BeginDistributionDate < _moneyDistributionSettings.FirstApril &&
                                 (x.Platform.DgppId == (int)PlatformEnum.Api || x.Platform.DgppId == (int)PlatformEnum.Online)) &&

                    // from branch to franch
                    ((x.DestOrganizationUnitId == organizationId &&
                     x.SourceOrganizationUnit.BranchOfficeOrganizationUnits.Any(
                         y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales
                              && y.BranchOffice.ContributionTypeId == (long)ContributionTypeEnum.Branch)) ||

                    // from franch to franch
                    (x.SourceOrganizationUnitId == organizationId &&
                     x.DestOrganizationUnit.BranchOfficeOrganizationUnits.Any(
                         y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales
                              && y.BranchOffice.ContributionTypeId == (long)ContributionTypeEnum.Franchisees))));
            }


            var orderInfos = orderInfosQuery.Select(x => new
            {
                Order = x,
                IsDgppOrder = x.Number.ToLower().StartsWith(RussianOf) || x.Number.ToLower().StartsWith(EnglishOf),
                HasLock = x.Locks.Any(y => !y.IsActive && !y.IsDeleted && y.PeriodStartDate >= startDate &&
                                                   y.PeriodEndDate <= endDate && y.DebitAccountDetailId != null &&
                                                   // Списание в счет оплаты БЗ
                                                   y.AccountDetail.OperationTypeId == (long)AccountOperationTypeEnum.WithdrawalForOrderPayment),
                SourceBou = x.SourceOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales)
                    .Select(y => new BranchOfficeDto
                    {
                        Id = y.Id,
                        ShortLegalName = y.ShortLegalName,
                        SyncCode1C = y.SyncCode1C,
                        Kpp = y.Kpp,
                        OrganizationUnitId = y.OrganizationUnitId,
                        OrganizationUnitSyncCode1C = y.OrganizationUnit.SyncCode1C,
                        BranchOfficeInn = y.BranchOffice.Inn,
                        BranchOfficeBargainTypeSyncCode1C = y.BranchOffice.BargainType.SyncCode1C,
                        BranchOfficeContributionType = (ContributionTypeEnum)y.BranchOffice.ContributionTypeId
                    }).FirstOrDefault(),
                DestBou = x.DestOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales)
                    .Select(y => new BranchOfficeDto
                    {
                        Id = y.Id,
                        ShortLegalName = y.ShortLegalName,
                        SyncCode1C = y.SyncCode1C,
                        Kpp = y.Kpp,
                        OrganizationUnitId = y.OrganizationUnitId,
                        OrganizationUnitSyncCode1C = y.OrganizationUnit.SyncCode1C,
                        BranchOfficeInn = y.BranchOffice.Inn,
                        BranchOfficeBargainTypeSyncCode1C = y.BranchOffice.BargainType.SyncCode1C,
                        BranchOfficeContributionType = (ContributionTypeEnum)y.BranchOffice.ContributionTypeId
                    }).FirstOrDefault()
            })
                .Where(x => !x.IsDgppOrder && x.HasLock)
                .Select(x => new
                {
                    OrderPositions = x.Order.OrderPositions.Where(y => y.IsActive && !y.IsDeleted && y.PricePosition.Cost > 0)
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
                    x.SourceBou,
                    x.DestBou,
                    LegalPersonOwnerCode = x.SourceBou.BranchOfficeContributionType == ContributionTypeEnum.Branch
                                           && x.DestBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees
                        ? x.Order.Account.LegalPerson.OwnerCode
                        : x.Order.LegalPerson.OwnerCode,
                    x.Order.Number,
                    x.Order.SignupDate,
                    x.Order.Id,
                    PlatformDgppId = x.Order.Platform.DgppId,
                    x.Order.DestOrganizationUnit.ElectronicMedia,
                    x.Order.BeginDistributionDate,
                })
                .Where(x => x.OrderPositions.Any())
                //.Where(x => x.Order.Id == 301724) // uncomment fo test
                .ToArray();


            var ukBou = _finder.Find<BranchOfficeOrganizationUnit>(x => x.Id == _moneyDistributionSettings.UkBouId).Select(x => new BranchOfficeDto
            {
                Id = x.Id,
                ShortLegalName = x.ShortLegalName,
                SyncCode1C = x.SyncCode1C,
                Kpp = x.Kpp,
                OrganizationUnitId = x.OrganizationUnitId,

                // при печати коммерция привыкла к коду 01 а не к 26
                OrganizationUnitSyncCode1C = "01",
                BranchOfficeInn = x.BranchOffice.Inn,
                BranchOfficeBargainTypeSyncCode1C = x.BranchOffice.BargainType.SyncCode1C,
                BranchOfficeContributionType = (ContributionTypeEnum)x.BranchOffice.ContributionTypeId
            }).Single();

            var blockingErrors = new List<OneCError>();
            var nonBlockingErrors = new List<OneCError>();
            var outputs = new List<OneCOutput>();

            foreach (var orderInfo in orderInfos)
            {
                decimal totalOrderCost = 0;
                var moneyDistributionCalculator = new MoneyDistributionCalculator(orderInfo.SourceBou.BranchOfficeContributionType,
                    orderInfo.DestBou.BranchOfficeContributionType,
                    orderInfo.SourceBou.Id == orderInfo.DestBou.Id,
                    orderInfo.BeginDistributionDate < _moneyDistributionSettings.FirstApril);

                foreach (var orderPosition in orderInfo.OrderPositions)
                {
                    var orderPositionDistributions = moneyDistributionCalculator.CalculatePlatformCosts(new OrderPositionDto
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

                    // в случае продажи между франчами считаем только то что ушло в УК
                    if (orderInfo.SourceBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees &&
                        orderInfo.DestBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees)
                    {
                        orderPositionDistributions = orderPositionDistributions.Where(x => x.NewTo == ESalesPointType.Uk);
                    }

                    var totalOrderPositionCost = orderPositionDistributions.Sum(x => x.DiscountCost);
                    totalOrderCost += totalOrderPositionCost;
                }

                if (totalOrderCost == 0)
                {
                    continue;
                }

                // подставляем обобщённое УК вместо реальных городов-источников\городов-назначений
                var legalPerson = (orderInfo.SourceBou.BranchOfficeContributionType == ContributionTypeEnum.Branch &&
                                   orderInfo.DestBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees)
                    ? ukBou
                    : orderInfo.SourceBou;
                var branchOffice = (orderInfo.SourceBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees &&
                                    orderInfo.DestBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees)
                    ? ukBou
                    : orderInfo.DestBou;

                var accountInfo = _finder.Find<Account>(x => x.IsActive && !x.IsDeleted)
                    .Where(x => x.BranchOfficeOrganizationUnit.BranchOffice.Inn == branchOffice.BranchOfficeInn &&
                                x.BranchOfficeOrganizationUnit.Kpp == branchOffice.Kpp &&
                                x.BranchOfficeOrganizationUnit.OrganizationUnitId == branchOffice.OrganizationUnitId)
                    .Where(x => (x.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.Businessman)
                        ? x.LegalPerson.Inn == legalPerson.BranchOfficeInn && x.LegalPerson.Kpp == null
                        : x.LegalPerson.Inn == legalPerson.BranchOfficeInn && x.LegalPerson.Kpp == legalPerson.Kpp)
                    .Select(x => new { x.LegalPerson, x.LegalPesonSyncCode1C })
                    .SingleOrDefault();

                if (accountInfo == null)
                {
                    blockingErrors.Add(new OneCError
                    {
                        LegalPersonId = 0,
                        SyncCode1C = null,
                        ErrorMessage =
                            string.Format(
                                "Не найден лицевой счёт между юр. лицом клиента '{0}' (ИНН={1}, КПП={2}) и юр. лицом исполнителя {3} (ИНН={4}, КПП={5}), по заказу '{6}'",
                                legalPerson.ShortLegalName, legalPerson.BranchOfficeInn, legalPerson.Kpp, branchOffice.ShortLegalName,
                                branchOffice.BranchOfficeInn, branchOffice.Kpp,
                                orderInfo.Number)
                    });

                    continue;
                }

                ValidateLegalPerson(accountInfo.LegalPerson, accountInfo.LegalPesonSyncCode1C, blockingErrors, nonBlockingErrors);

                outputs.Add(new OneCOutput
                {
                    BargainType = branchOffice.BranchOfficeBargainTypeSyncCode1C,
                    BranchOfficeOUCode = branchOffice.SyncCode1C,
                    LegalPersonSynCode = accountInfo.LegalPesonSyncCode1C,
                    Inn = accountInfo.LegalPerson.Inn,
                    Kpp = accountInfo.LegalPerson.Kpp,
                    Number = CreateOrderNumber(legalPerson.OrganizationUnitSyncCode1C, branchOffice.OrganizationUnitSyncCode1C, startDate),
                    SignupDate = orderInfo.SignupDate.ToString("dd.MM.yy"),
                    Sum = Math.Round(totalOrderCost, 2, MidpointRounding.ToEven).ToString(),
                    LegalName = accountInfo.LegalPerson.LegalName,
                    ShortName = accountInfo.LegalPerson.ShortName,
                    OwnerDisplayName = _userIdentifierService.GetUserInfo(orderInfo.LegalPersonOwnerCode).DisplayName,
                    ElectronicMedia = orderInfo.ElectronicMedia,
                    DgppId = orderInfo.PlatformDgppId,
                    OrderNumber = orderInfo.Number
                });
            }

            var response = new ExportAccountDetailsTo1CResponse
            {
                BlockingErrorsAmount = blockingErrors.Count,
                NonBlockingErrorsAmount = nonBlockingErrors.Count,
                ErrorFile =
                    new FileDescription
                    {
                        FileName = "ExporErrors_" + DateTime.Today.ToShortDateString() + ".csv",
                        ContentType = MediaTypeNames.Application.Octet,
                        Stream = ToOneCStream(blockingErrors.Concat(nonBlockingErrors))
                    }
            };

            if (blockingErrors.Any())
            {
                return response;
            }

            response.File = new FileDescription { FileName = "Acts.csv", ContentType = MediaTypeNames.Application.Octet, Stream = ToOneCStream(outputs), };
            response.ProcessedWithoutErrors = outputs.Count - (blockingErrors.Count + nonBlockingErrors.Count);

            return response;
        }

        public ExportAccountDetailsTo1CResponse ExportAccountDetailsToServiceBus(long organizationUnitId, DateTime startDate, DateTime endDate)
        {
            _useCaseTuner.AlterDuration<AccountingSystemService>();

            var contributionType = _branchOfficeRepository.GetContributionTypeForOrganizationUnit(organizationUnitId);

            var orgUnitSyncCode = _finder.Find<OrganizationUnit>(x => x.Id == organizationUnitId).Select(x => x.SyncCode1C).Single();

            var orderInfosQuery = _finder.Find<Order>(x => !x.IsDeleted);

            // кастомизуем запрос в зависимости от формы сотрудничества
            switch (contributionType)
            {
                case ContributionTypeEnum.Branch:
                    {
                        orderInfosQuery = orderInfosQuery.Where(x => x.DestOrganizationUnitId == organizationUnitId);
                        if (startDate >= _moneyDistributionSettings.FirstApril)
                        {
                            orderInfosQuery = orderInfosQuery.Where(x => x.BeginDistributionDate >= _moneyDistributionSettings.FirstApril);
                        }

                        break;
                    }

                case ContributionTypeEnum.Franchisees:
                    {
                        orderInfosQuery = orderInfosQuery
                            .Where(x => !(x.BeginDistributionDate < _moneyDistributionSettings.FirstApril &&
                                          (x.Platform.DgppId == (int)PlatformEnum.Api || x.Platform.DgppId == (int)PlatformEnum.Online)) &&

                                        // from branch to franch
                                        ((x.DestOrganizationUnitId == organizationUnitId &&
                                          x.SourceOrganizationUnit.BranchOfficeOrganizationUnits.Any(y => y.IsActive && !y.IsDeleted
                                                                                                          && y.IsPrimaryForRegionalSales &&
                                                                                                          y.BranchOffice.ContributionTypeId ==
                                                                                                          (long)ContributionTypeEnum.Branch)) ||

                                         // from franch to franch
                                         (x.SourceOrganizationUnitId == organizationUnitId &&
                                          x.DestOrganizationUnit.BranchOfficeOrganizationUnits.Any(y => y.IsActive && !y.IsDeleted
                                                                                                        && y.IsPrimaryForRegionalSales &&
                                                                                                        y.BranchOffice.ContributionTypeId ==
                                                                                                        (long)ContributionTypeEnum.Franchisees))));
                        break;
                    }
            }


            var orderInfos = orderInfosQuery.Select(x => new
            {
                Order = x,
                IsDgppOrder = x.Number.ToLower().StartsWith(RussianOf) || x.Number.ToLower().StartsWith(EnglishOf),
                HasLock = x.Locks.Any(y => !y.IsActive && !y.IsDeleted && y.PeriodStartDate >= startDate &&
                                                   y.PeriodEndDate <= endDate && y.DebitAccountDetailId != null &&
                                                   y.AccountDetail.OperationTypeId == (long)AccountOperationTypeEnum.WithdrawalForOrderPayment),
                SourceBou = x.SourceOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales)
                    .Select(y => new BranchOfficeDto
                    {
                        Id = y.Id,
                        ShortLegalName = y.ShortLegalName,
                        SyncCode1C = y.SyncCode1C,
                        Kpp = y.Kpp,
                        OrganizationUnitId = y.OrganizationUnitId,
                        OrganizationUnitSyncCode1C = y.OrganizationUnit.SyncCode1C,
                        BranchOfficeInn = y.BranchOffice.Inn,
                        BranchOfficeBargainTypeSyncCode1C = y.BranchOffice.BargainType.SyncCode1C,
                        BranchOfficeContributionType = (ContributionTypeEnum)y.BranchOffice.ContributionTypeId
                    }).FirstOrDefault(),
                DestBou = x.DestOrganizationUnit.BranchOfficeOrganizationUnits.Where(y => y.IsActive && !y.IsDeleted && y.IsPrimaryForRegionalSales)
                    .Select(y => new BranchOfficeDto
                    {
                        Id = y.Id,
                        ShortLegalName = y.ShortLegalName,
                        SyncCode1C = y.SyncCode1C,
                        Kpp = y.Kpp,
                        OrganizationUnitId = y.OrganizationUnitId,
                        OrganizationUnitSyncCode1C = y.OrganizationUnit.SyncCode1C,
                        BranchOfficeInn = y.BranchOffice.Inn,
                        BranchOfficeBargainTypeSyncCode1C = y.BranchOffice.BargainType.SyncCode1C,
                        BranchOfficeContributionType = (ContributionTypeEnum)y.BranchOffice.ContributionTypeId
                    }).FirstOrDefault()
            })
                .Where(x => !x.IsDgppOrder && x.HasLock)
                .Select(x => new
                {
                    OrderPositions = x.Order.OrderPositions.Where(y => y.IsActive && !y.IsDeleted && y.PricePosition.Cost > 0)
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
                    x.SourceBou,
                    x.DestBou,
                    LegalPersonOwnerCode = x.SourceBou.BranchOfficeContributionType == ContributionTypeEnum.Branch
                                           && x.DestBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees
                        ? x.Order.Account.LegalPerson.OwnerCode
                        : x.Order.LegalPerson.OwnerCode,
                    x.Order.Number,
                    x.Order.RegionalNumber,
                    x.Order.SignupDate,
                    x.Order.Id,
                    PlatformDgppId = x.Order.Platform.DgppId,
                    x.Order.DestOrganizationUnit.ElectronicMedia,
                    x.Order.BeginDistributionDate,
                })
                .Where(x => x.OrderPositions.Any())
                .ToArray();


            var ukBou = _finder.Find<BranchOfficeOrganizationUnit>(x => x.Id == _moneyDistributionSettings.UkBouId).Select(x => new BranchOfficeDto
            {
                Id = x.Id,
                ShortLegalName = x.ShortLegalName,
                SyncCode1C = x.SyncCode1C,
                Kpp = x.Kpp,
                OrganizationUnitId = x.OrganizationUnitId,

                // при печати коммерция привыкла к коду 01 а не к 26
                OrganizationUnitSyncCode1C = "01",
                BranchOfficeInn = x.BranchOffice.Inn,
                BranchOfficeBargainTypeSyncCode1C = x.BranchOffice.BargainType.SyncCode1C,
                BranchOfficeContributionType = (ContributionTypeEnum)x.BranchOffice.ContributionTypeId
            }).Single();

            var blockingErrors = new List<OneCError>();
            var nonBlockingErrors = new List<OneCError>();
            var debits = new List<DebitDto>();

            foreach (var orderInfo in orderInfos)
            {
                decimal totalOrderCost = 0;
                var totalCostByPlatform = _extendedPlatformList.ToDictionary(x => x, _ => 0m);


                var moneyDistributionCalculator = new MoneyDistributionCalculator(orderInfo.SourceBou.BranchOfficeContributionType,
                    orderInfo.DestBou.BranchOfficeContributionType,
                    orderInfo.SourceBou.Id == orderInfo.DestBou.Id,
                    orderInfo.BeginDistributionDate < _moneyDistributionSettings.FirstApril);

                foreach (var orderPosition in orderInfo.OrderPositions)
                {
                    var orderPositionDistributions = moneyDistributionCalculator.CalculatePlatformCosts(new OrderPositionDto
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

                    // в случае продажи между франчами считаем только то что ушло в УК
                    if (orderInfo.SourceBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees &&
                        orderInfo.DestBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees)
                    {
                        orderPositionDistributions = orderPositionDistributions.Where(x => x.NewTo == ESalesPointType.Uk);
                    }

                    var totalOrderPositionCost = orderPositionDistributions.Sum(x => x.DiscountCost);

                    foreach (var platform in _extendedPlatformList)
                    {
                        var p = platform;
                        totalCostByPlatform[platform] += orderPositionDistributions.Where(x => x.Platform == p).Sum(x => x.DiscountCost);
                    }

                    totalOrderCost += totalOrderPositionCost;
                }

                if (totalOrderCost == 0)
                {
                    continue;
                }

                // подставляем обобщённое УК вместо реальных городов-источников\городов-назначений
                var legalPerson = (orderInfo.SourceBou.BranchOfficeContributionType == ContributionTypeEnum.Branch &&
                                   orderInfo.DestBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees)
                    ? ukBou
                    : orderInfo.SourceBou;
                var branchOffice = (orderInfo.SourceBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees &&
                                    orderInfo.DestBou.BranchOfficeContributionType == ContributionTypeEnum.Franchisees)
                    ? ukBou
                    : orderInfo.DestBou;

                var accountInfo = _finder.Find<Account>(x => x.IsActive && !x.IsDeleted)
                    .Where(x => x.BranchOfficeOrganizationUnit.BranchOffice.Inn == branchOffice.BranchOfficeInn &&
                                x.BranchOfficeOrganizationUnit.Kpp == branchOffice.Kpp &&
                                x.BranchOfficeOrganizationUnit.OrganizationUnitId == branchOffice.OrganizationUnitId)
                    .Where(x => (x.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.Businessman)
                        ? x.LegalPerson.Inn == legalPerson.BranchOfficeInn && x.LegalPerson.Kpp == null
                        : x.LegalPerson.Inn == legalPerson.BranchOfficeInn && x.LegalPerson.Kpp == legalPerson.Kpp)
                    .Select(x => new
                    {
                        x.Id,
                        x.LegalPerson,
                        x.LegalPesonSyncCode1C,
                        ProfileCode = x.LegalPerson.LegalPersonProfiles.Where(p => !p.IsDeleted && p.IsMainProfile).Select(p => p.Id).FirstOrDefault()
                    })
                    .SingleOrDefault();

                if (accountInfo == null)
                {
                    blockingErrors.Add(new OneCError
                    {
                        LegalPersonId = 0,
                        SyncCode1C = null,
                        ErrorMessage =
                            string.Format(
                                "Не найден лицевой счёт между юр. лицом клиента '{0}' (ИНН={1}, КПП={2}) и юр. лицом исполнителя {3} (ИНН={4}, КПП={5}), по заказу '{6}'",
                                legalPerson.ShortLegalName, legalPerson.BranchOfficeInn, legalPerson.Kpp, branchOffice.ShortLegalName,
                                branchOffice.BranchOfficeInn, branchOffice.Kpp,
                                orderInfo.Number)
                    });

                    continue;
                }

                ValidateLegalPerson(accountInfo.LegalPerson, accountInfo.LegalPesonSyncCode1C, blockingErrors, nonBlockingErrors);

                debits.Add(new DebitDto
                {
                    AccountCode = accountInfo.Id,
                    Amount = Math.Round(totalOrderCost, 2, MidpointRounding.ToEven),
                    ClientOrderNumber = orderInfo.Number,
                    MediaInfo = orderInfo.ElectronicMedia,
                    Type = DebitDto.DebitType.Regional,

                    // Выгрука в формате csv не использует поле RegionalNumber
                    OrderNumber = CreateOrderNumber(legalPerson.OrganizationUnitSyncCode1C, branchOffice.OrganizationUnitSyncCode1C, startDate),
                    ProfileCode = accountInfo.ProfileCode,
                    SignupDate = orderInfo.SignupDate,
                    LegalEntityBranchCode1C = branchOffice.SyncCode1C,
                    PlatformDistributions = new[]
                            {
                                new PlatformDistribution { Amount = totalCostByPlatform[PlatformsExtended.Desktop],  PlatformCode = PlatformEnum.Desktop },
                                new PlatformDistribution { Amount = totalCostByPlatform[PlatformsExtended.Mobile],  PlatformCode = PlatformEnum.Mobile },

                                // Платформа Api может разбваться на ApiOnline + ApiPartner
                                new PlatformDistribution { Amount = totalCostByPlatform[PlatformsExtended.ApiOnline] + totalCostByPlatform[PlatformsExtended.ApiPartner] + totalCostByPlatform[PlatformsExtended.Api],  PlatformCode = PlatformEnum.Api },
                                new PlatformDistribution { Amount = totalCostByPlatform[PlatformsExtended.Online],  PlatformCode = PlatformEnum.Online }
                            }
                });
            }

            return new ExportAccountDetailsTo1CResponse
            {
                File = blockingErrors.Any()
                                 ? null
                                 : new FileDescription
                                 {
                                     FileName = "DebitsInfo_" + DateTime.Today.ToShortDateString() + ".xml",
                                     ContentType = MediaTypeNames.Text.Xml,
                                     Stream = ToXmlStream(new DebitsInfoDto
                                     {
                                         StartDate = startDate,
                                         EndDate = endDate,
                                         OrganizationUnitCode = orgUnitSyncCode,
                                         Debits = debits.ToArray()
                                     })
                                 },
                BlockingErrorsAmount = blockingErrors.Count,
                NonBlockingErrorsAmount = nonBlockingErrors.Count,
                ErrorFile = blockingErrors.Any() || nonBlockingErrors.Any()
                                ? new FileDescription
                                {
                                    FileName = "ExportErrors_" + DateTime.Today.ToShortDateString() + ".csv",
                                    ContentType = MediaTypeNames.Application.Octet,
                                    Stream = ToOneCStream(blockingErrors.Concat(nonBlockingErrors))
                                }
                                : null,
                ProcessedWithoutErrors = debits.Count - (blockingErrors.Count + nonBlockingErrors.Count)
            };
        }

        private void ValidateLegalPerson(LegalPerson legalPerson,
            string syncCode1C,
            ICollection<OneCError> blockingErrors,
            ICollection<OneCError> nonBlockingErrors)
        {
            var oneCError = new OneCError { LegalPersonId = legalPerson.Id, SyncCode1C = syncCode1C, };

            if (string.IsNullOrEmpty(syncCode1C))
            {
                oneCError.ErrorMessage = "Не указан идентификатор 1С";
                blockingErrors.Add(oneCError);
            }

            if (string.IsNullOrEmpty(legalPerson.LegalName))
            {
                oneCError.ErrorMessage = "Не указано юридическое название";
                blockingErrors.Add(oneCError);
            }

            if (string.IsNullOrEmpty(legalPerson.ShortName))
            {
                oneCError.ErrorMessage = "Не указано короткое название";
                blockingErrors.Add(oneCError);
            }

            if (string.IsNullOrEmpty(legalPerson.LegalAddress))
            {
                oneCError.ErrorMessage = "Не указан юридический адрес";
                blockingErrors.Add(oneCError);
            }

            switch ((LegalPersonType)legalPerson.LegalPersonTypeEnum)
            {
                case LegalPersonType.LegalPerson:
                {
                    if (string.IsNullOrEmpty(legalPerson.Inn))
                    {
                        oneCError.ErrorMessage = "Не указан ИНН";
                        blockingErrors.Add(oneCError);
                    }
                    else if (legalPerson.Inn.Replace(" ", string.Empty).Length != 10)
                    {
                        oneCError.ErrorMessage = "ИНН не соответствует формату";
                        blockingErrors.Add(oneCError);
                    }

                    if (string.IsNullOrEmpty(legalPerson.Kpp))
                    {
                        oneCError.ErrorMessage = "Не указан КПП";
                        blockingErrors.Add(oneCError);
                    }
                    else if (legalPerson.Kpp.Replace(" ", string.Empty).Length != 9)
                    {
                        oneCError.ErrorMessage = "КПП не соответствует формату";
                        blockingErrors.Add(oneCError);
                    }
                }
                    break;

                case LegalPersonType.Businessman:
                {
                    if (string.IsNullOrEmpty(legalPerson.Inn))
                    {
                        oneCError.ErrorMessage = "Не указан ИНН";
                        blockingErrors.Add(oneCError);
                    }
                    else if (legalPerson.Inn.Replace(" ", string.Empty).Length != 12)
                    {
                        oneCError.ErrorMessage = "ИНН не соответствует формату";
                        blockingErrors.Add(oneCError);
                    }
                }
                    break;
            }

            // Код 1С [{0}] юр лица клиента не уникален
            var syncCode1CCount = _finder.FindAll<Account>().Count(x => x.LegalPesonSyncCode1C == syncCode1C);
            if (syncCode1CCount > 1)
            {
                oneCError.ErrorMessage = string.Format("Код 1С [{0}] юр лица клиента не уникален", syncCode1C);
                nonBlockingErrors.Add(oneCError);
            }
        }

        private static string CreateOrderNumber(string sourceSyncCode1C, string destSyncCode1C, DateTime startDate)
        {
            return string.Format("{0}-{1}-{2}-{3}", sourceSyncCode1C, destSyncCode1C, startDate.ToString("MM"), startDate.ToString("yy"));
        }

        private static byte[] ToOneCStream(IEnumerable<OneCError> errors)
        {
            var table = new DataTable();
            table.Columns.Add("LegalPersonId");
            table.Columns.Add("SyncCode1C");
            table.Columns.Add("ErrorMessage");

            foreach (var error in errors)
            {
                table.Rows.Add(error.LegalPersonId, error.SyncCode1C, error.ErrorMessage);
            }

            return Encoding.GetEncoding(1251).GetBytes(table.ToCsvEscaped(';', false));
        }

        private static byte[] ToOneCStream(IEnumerable<OneCOutput> records)
        {
            var table = new DataTable();
            table.Columns.Add("BargainType");
            table.Columns.Add("BranchOfficeOUCode");
            table.Columns.Add("LegalPersonSynCode");
            table.Columns.Add("Inn");
            table.Columns.Add("Kpp");
            table.Columns.Add("Number");
            table.Columns.Add("SignupDate");
            table.Columns.Add("Sum");
            table.Columns.Add("LegalName");
            table.Columns.Add("ShortName");
            table.Columns.Add("OwnerDisplayName");
            table.Columns.Add("ElectronicMedia");
            table.Columns.Add("DgppId");
            table.Columns.Add("OrderNumber");

            foreach (var record in records)
            {
                table.Rows.Add(record.BargainType, record.BranchOfficeOUCode, record.LegalPersonSynCode, record.Inn, record.Kpp, record.Number,
                    record.SignupDate, record.Sum, record.LegalName, record.ShortName, record.OwnerDisplayName, record.ElectronicMedia, record.DgppId,
                    record.OrderNumber);
            }

            return Encoding.GetEncoding(1251).GetBytes(table.ToCsvEscaped(';', false));
        }

        private static byte[] ToXmlStream(DebitsInfoDto infoDto)
        {
            return Encoding.UTF8.GetBytes(infoDto.ToXElement().ToString(SaveOptions.None));
        }

        private sealed class BranchOfficeDto
        {
            public long Id { get; set; }
            public string ShortLegalName { get; set; }
            public string SyncCode1C { get; set; }
            public string Kpp { get; set; }
            public long OrganizationUnitId { get; set; }
            public string OrganizationUnitSyncCode1C { get; set; }
            public string BranchOfficeInn { get; set; }
            public string BranchOfficeBargainTypeSyncCode1C { get; set; }
            public ContributionTypeEnum BranchOfficeContributionType { get; set; }
        }

        private sealed class OneCError
        {
            public long LegalPersonId;
            public string SyncCode1C;
            public string ErrorMessage;
        }

        private sealed class OneCOutput
        {
            public string BargainType;

            public string BranchOfficeOUCode;

            public string LegalPersonSynCode;

            public string Inn;

            public string Kpp;

            public string Number;

            public string SignupDate;

            public string Sum;

            public string LegalName;

            public string ShortName;

            public string OwnerDisplayName;

            public string ElectronicMedia;

            public long DgppId;

            public string OrderNumber;
        }
    }
}