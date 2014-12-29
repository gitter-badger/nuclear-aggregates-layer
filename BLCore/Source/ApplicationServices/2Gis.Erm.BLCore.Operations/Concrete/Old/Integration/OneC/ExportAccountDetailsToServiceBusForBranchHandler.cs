using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using SaveOptions = System.Xml.Linq.SaveOptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class ExportAccountDetailsToServiceBusForBranchHandler : RequestHandler<ExportAccountDetailsToServiceBusForBranchRequest, IntegrationResponse>
    {
        private const string RussianhOf = "оф";
        private const string EnglishOf = "oф";

        private const decimal Accuracy = 1M;

        private static readonly Encoding CyrillicEncoding = Encoding.GetEncoding(1251);
        private static readonly PlatformEnum[] NonAllowedPlatforms = { PlatformEnum.Api, PlatformEnum.Online };
        private static readonly DateTime FirstOfApril2013 = new DateTime(2013, 4, 1);

        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IGlobalizationSettings _globalizationSettings;
        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IUseCaseTuner _useCaseTuner;

        public ExportAccountDetailsToServiceBusForBranchHandler(
            IFinder finder,
            ISubRequestProcessor subRequestProcessor,
            IOrderReadModel orderReadModel,
            IGlobalizationSettings globalizationSettings,
            IBusinessModelSettings businessModelSettings,
            IUseCaseTuner useCaseTuner)
        {
            _finder = finder;
            _subRequestProcessor = subRequestProcessor;
            _orderReadModel = orderReadModel;
            _globalizationSettings = globalizationSettings;
            _businessModelSettings = businessModelSettings;
            _useCaseTuner = useCaseTuner;
        }

        private enum ExportOrderType
        {
            None = 0,
            LocalAndOutgoing = 1,
            IncomingFromFranchiseesClient = 2,
            IncomingFromFranchiseesDgpp = 3
        }

        protected override IntegrationResponse Handle(ExportAccountDetailsToServiceBusForBranchRequest request)
        {
            _useCaseTuner.AlterDuration<ExportAccountDetailsToServiceBusForBranchHandler>();

            var period = new TimePeriod(request.StartPeriodDate, request.EndPeriodDate);

            var organizationUnitSyncCode1C = _finder.FindAll<OrganizationUnit>()
                                                    .Where(x => x.Id == request.OrganizationUnitId && x.ErmLaunchDate != null)
                                                    .SelectMany(x => x.BranchOfficeOrganizationUnits)
                                                    .Where(x => x.IsPrimary && x.BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Branch)
                                                    .Select(x => x.OrganizationUnit.SyncCode1C)
                                                    .FirstOrDefault();

            if (organizationUnitSyncCode1C == null)
            {
                throw new NotificationException(BLResources.SelectedOrganizationUnitIsNotBranchOrNotMovedToERM);
            }

            var accountDetailDtos = GetAccountDetailDtos(request.OrganizationUnitId, period);

            var dtosToValidate = accountDetailDtos.Where(x => x.LegalPersonSyncCode1C != null).DistinctBy(x => x.LegalPersonSyncCode1C).ToArray();
            var validateLegalPersonsResponse = ValidateLegalPersons(dtosToValidate);

            var orderIds = accountDetailDtos.Select(x => x.OrderId).Distinct().ToArray();
            var distributions = _orderReadModel.GetOrderPlatformDistributions(orderIds, period.Start, period.End);
            foreach (var accountDetailDto in accountDetailDtos)
            {
                accountDetailDto.PlatformDistributions = distributions[accountDetailDto.OrderId];
            }

            var blockingErrors = EvaluateAllBlockingErrors(accountDetailDtos, validateLegalPersonsResponse.BlockingErrors);

            var debitInfoDto = ConvertToDebitsInfoInitialDto(organizationUnitSyncCode1C, period, accountDetailDtos);
            var debitsStream = CreateDebitsStream(debitInfoDto.ToXElement());

            var response = ConstructResponse(blockingErrors,
                                             validateLegalPersonsResponse.NonBlockingErrors,
                                             debitInfoDto.Debits.Length,
                                             debitsStream);
            return response;
        }

        private static MemoryStream CreateDebitsStream(XElement debitsXml)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(debitsXml.ToString(SaveOptions.None)));
            return stream;
        }

        private static DataTable GetErrorsDataTable(IEnumerable<ErrorDto> blockingErrors, IEnumerable<ErrorDto> nonBlockingErrors)
        {
            const int AttributesCount = 4;
            var dataTable = new DataTable { Locale = CultureInfo.InvariantCulture };
            for (var i = 0; i < AttributesCount; i++)
            {
                dataTable.Columns.Add(string.Empty);
            }

            foreach (var error in blockingErrors)
            {
                dataTable.Rows.Add(error.LegalPersonId, error.SyncCode1C, BLResources.BlockingError, error.ErrorMessage);
            }

            foreach (var error in nonBlockingErrors)
            {
                dataTable.Rows.Add(error.LegalPersonId, error.SyncCode1C, BLResources.NonBlockingError, error.ErrorMessage);
            }

            return dataTable;
        }

        private DebitsInfoInitialDto ConvertToDebitsInfoInitialDto(string organizationUnitSyncCode1C,
                                                                   TimePeriod period,
                                                                   IEnumerable<AccountDetailDto> accountDetailDtos)
        {
            var debits = accountDetailDtos
                .Select(x => new DebitDto
                                 {
                                     OrderCode = x.OrderId,
                                     AccountCode = x.AccountCode,
                                     ProfileCode = x.ProfileCode,
                                     Amount = GetAccountDetailAmount(x),
                                     SignupDate = x.OrderSignupDateUtc,
                                     ClientOrderNumber = x.ClientOrderNumber,
                                     OrderType = (int)x.OrderType,
                                     OrderNumber = x.OrderNumber,
                                     MediaInfo = x.ElectronicMedia,
                                     LegalEntityBranchCode1C = x.BranchOfficeOrganizationUnitSyncCode1C,
                                     Type = x.Type == ExportOrderType.LocalAndOutgoing || x.Type == ExportOrderType.IncomingFromFranchiseesDgpp
                                                ? DebitDto.DebitType.Client
                                                : DebitDto.DebitType.Regional,
                                     PlatformDistributions = new[]
                                                                 {
                                                                     new PlatformDistribution
                                                                         {
                                                                             PlatformCode = PlatformEnum.Desktop,
                                                                             Amount = x.PlatformDistributions.ContainsKey(PlatformEnum.Desktop)
                                                                                          ? x.PlatformDistributions[PlatformEnum.Desktop]
                                                                                          : 0
                                                                         },
                                                                     new PlatformDistribution
                                                                         {
                                                                             PlatformCode = PlatformEnum.Mobile,
                                                                             Amount = x.PlatformDistributions.ContainsKey(PlatformEnum.Mobile)
                                                                                          ? x.PlatformDistributions[PlatformEnum.Mobile]
                                                                                          : 0
                                                                         },
                                                                     new PlatformDistribution
                                                                         {
                                                                             PlatformCode = PlatformEnum.Api,
                                                                             Amount = x.PlatformDistributions.ContainsKey(PlatformEnum.Api)
                                                                                          ? x.PlatformDistributions[PlatformEnum.Api]
                                                                                          : 0
                                                                         },
                                                                     new PlatformDistribution
                                                                         {
                                                                             PlatformCode = PlatformEnum.Online,
                                                                             Amount = x.PlatformDistributions.ContainsKey(PlatformEnum.Online)
                                                                                          ? x.PlatformDistributions[PlatformEnum.Online]
                                                                                          : 0
                                                                         }
                                                                 }
                                 })
                .ToArray();

            return new DebitsInfoInitialDto
                       {
                           OrganizationUnitCode = organizationUnitSyncCode1C,
                           StartDate = period.Start,
                           EndDate = period.End,
                           ClientDebitTotalAmount = debits.Where(x => x.Type == DebitDto.DebitType.Client).Sum(x => x.Amount),
                           Debits = debits
                       };
        }

        private decimal GetAccountDetailAmount(AccountDetailDto accountDetailDto)
        {
            const decimal FranchiseesAmountFactor = 0.4m;
            const decimal VatMultiplier = 0.18m;
            if (accountDetailDto.Type == ExportOrderType.LocalAndOutgoing ||
                accountDetailDto.Type == ExportOrderType.IncomingFromFranchiseesDgpp)
            {
                // Исходящий или
                // региональный БЗ из ДГПП (Номер заказа начинается на "ОФ") -> 
                // Списание берём 100% из соответствующей операции с лицевым счётом.
                return accountDetailDto.DebitAccountDetailAmount;
            }

            // - клиентский БЗ созданный в ERM либо ДГПП (номер заказа НЕ начинается на "ОФ")
            // -> Списание берём 40% из соответствующей операции с лицевым 
            // счётом и добавляем к этой сумме НДС;
            return Math.Round(FranchiseesAmountFactor * accountDetailDto.DebitAccountDetailAmount * VatMultiplier, _businessModelSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
        }

        private IntegrationResponse ConstructResponse(IReadOnlyCollection<ErrorDto> blockingErrors,
                                                      IReadOnlyCollection<ErrorDto> nonBlockingErrors,
                                                      int debitsCount,
                                                      Stream debitsStream)
        {
            var errorsFileName = string.Format("ExportErrors_{0}.csv", DateTime.Today.ToShortDateString());
            var response = new IntegrationResponse
            {
                BlockingErrorsAmount = blockingErrors.Count,
                NonBlockingErrorsAmount = nonBlockingErrors.Count
            };

            var errorContent = GetErrorsDataTable(blockingErrors, nonBlockingErrors).ToCsv(_globalizationSettings.ApplicationCulture.TextInfo.ListSeparator);
            if (blockingErrors.Any())
            {
                response.FileName = errorsFileName;
                response.ContentType = MediaTypeNames.Application.Octet;
                response.Stream = new MemoryStream(CyrillicEncoding.GetBytes(errorContent));

                return response;
            }

            var streamDictionary = new Dictionary<string, Stream>();
            if (nonBlockingErrors.Any())
            {
                streamDictionary.Add(errorsFileName, new MemoryStream(CyrillicEncoding.GetBytes(errorContent)));
            }

            streamDictionary.Add("DebitsInfoInitial_" + DateTime.Today.ToShortDateString() + ".xml", debitsStream);

            response.FileName = "Acts.zip";
            response.ContentType = MediaTypeNames.Application.Zip;
            response.Stream = streamDictionary.ZipStreamDictionary();
            response.ProcessedWithoutErrors = debitsCount - (blockingErrors.Count + nonBlockingErrors.Count);

            return response;
        }

        private AccountDetailDto[] GetAccountDetailDtos(long organizationUnitId, TimePeriod period)
        {
            var accountDetailsQuery = _finder.FindAll<AccountDetail>();

            var allWithoutLegalPerson = (from @lock in _finder.FindAll<Lock>()
                                         let type = @lock.Order.SourceOrganizationUnitId == organizationUnitId
                                                        ? ExportOrderType.LocalAndOutgoing
                                                        : @lock.Order.DestOrganizationUnitId == organizationUnitId &&
                                                          @lock.Order.SourceOrganizationUnit.BranchOfficeOrganizationUnits
                                                               .FirstOrDefault(x => x.IsPrimary)
                                                               .BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Franchisees

                                                              // старая логика работает только для заказов размещающихся до 1 апреля
                                                              ? (@lock.Order.Number.ToLower().StartsWith(RussianhOf) || @lock.Order.Number.ToLower().StartsWith(EnglishOf)
                                                                     ? ExportOrderType.IncomingFromFranchiseesDgpp
                                                                     : (@lock.Order.BeginDistributionDate < FirstOfApril2013 &&
                                                                        !NonAllowedPlatforms.Contains((PlatformEnum)@lock.Order.Platform.DgppId))
                                                                           ? ExportOrderType.IncomingFromFranchiseesClient
                                                                           : ExportOrderType.None)

                                                              : ExportOrderType.None
                                         where !@lock.IsActive && !@lock.IsDeleted &&
                                               @lock.PeriodStartDate == period.Start && @lock.PeriodEndDate == period.End &&
                                               type != ExportOrderType.None
                                         select new
                                         {
                                             Lock = @lock,
                                             Type = type
                                         })
                .Select(x => x.Type == ExportOrderType.LocalAndOutgoing ||
                             x.Type == ExportOrderType.IncomingFromFranchiseesDgpp
                                 ? new
                                 {
                                     AccountDetailDto = new AccountDetailDto
                                     {
                                         OrderHasPositionsWithPlannedProvision =
                                             x.Lock.Order.OrderPositions.Any(op => op.IsActive
                                                                                   && !op.IsDeleted
                                                                                   &&
                                                                                   op.PricePosition.Position.SalesModel ==
                                                                                   SalesModel.PlannedProvision),
                                         BranchOfficeOrganizationUnitSyncCode1C = x.Lock.Account.BranchOfficeOrganizationUnit.SyncCode1C,
                                         AccountCode = x.Lock.Account.Id,
                                         ProfileCode = x.Lock.Order.LegalPersonProfileId != null
                                                           ? x.Lock.Order.LegalPersonProfile.Id
                                                           : x.Lock.Account.LegalPerson.LegalPersonProfiles
                                                              .Where(p => !p.IsDeleted && p.IsMainProfile)
                                                              .Select(p => p.Id)
                                                              .FirstOrDefault(),
                                         ClientOrderNumber = null,
                                         LegalPersonSyncCode1C = x.Lock.Account.LegalPesonSyncCode1C,
                                         OrderType = x.Lock.Order.OrderType,
                                         OrderNumber = x.Lock.Order.Number,
                                         OrderSignupDateUtc = x.Lock.Order.SignupDate,
                                         DebitAccountDetailAmount = accountDetailsQuery.Where(y => y.Id == x.Lock.DebitAccountDetailId)
                                                                                       .Select(y => y.Amount)
                                                                                       .FirstOrDefault(),
                                         ElectronicMedia = x.Lock.Order.DestOrganizationUnit.ElectronicMedia,
                                         OrderId = x.Lock.OrderId,
                                         Type = x.Type,
                                     },

                                     LegalPersonId = x.Lock.Account.LegalPersonId,
                                 }

                                 // ExportOrderType.IncomingFromFranchiseesClient
                                 : new
                                 {
                                     AccountDetailDto = new AccountDetailDto
                                     {
                                         OrderHasPositionsWithPlannedProvision = false,

                                         // Поля заполняются ниже из extension 
                                         BranchOfficeOrganizationUnitSyncCode1C = string.Empty,
                                         AccountCode = 0,
                                         ProfileCode = 0,
                                         ClientOrderNumber = x.Lock.Order.Number,
                                         LegalPersonSyncCode1C = x.Lock.Account.LegalPesonSyncCode1C,
                                         OrderType = x.Lock.Order.OrderType,
                                         OrderNumber = x.Lock.Order.RegionalNumber,
                                         OrderSignupDateUtc = x.Lock.Order.SignupDate,
                                         DebitAccountDetailAmount = accountDetailsQuery.Where(y => y.Id == x.Lock.DebitAccountDetailId)
                                                                                       .Select(y => y.Amount)
                                                                                       .FirstOrDefault(),
                                         ElectronicMedia = x.Lock.Order.DestOrganizationUnit.ElectronicMedia,
                                         OrderId = x.Lock.OrderId,
                                         Type = x.Type,
                                     },

                                     LegalPersonId = x.Lock.Account.LegalPersonId,
                                 })
                .Where(x => x.AccountDetailDto.DebitAccountDetailAmount > 0)
                .ToArray();

            var legalPersons = _finder.FindMany(Specs.Find.ByIds<LegalPerson>(allWithoutLegalPerson.Select(x => x.LegalPersonId)))
                                      .ToDictionary(x => x.Id, x => x);

            return allWithoutLegalPerson
                .Select(x =>
                {
                    x.AccountDetailDto.LegalPerson = legalPersons[x.LegalPersonId];
                    return x.AccountDetailDto;
                })
                .ToArray();
        }

        private ValidateLegalPersonsResponse ValidateLegalPersons(IEnumerable<AccountDetailDto> allAccountDetails)
        {
            var request = new ValidateLegalPersonsFor1CRequest
            {
                Entities = allAccountDetails.Select(x => new ValidateLegalPersonRequestItem
                {
                    Entity = x.LegalPerson,
                    SyncCode1C = x.LegalPersonSyncCode1C
                })
            };

            return (ValidateLegalPersonsResponse)_subRequestProcessor.HandleSubRequest(request, Context);
        }

        private List<ErrorDto> EvaluateAllBlockingErrors(IEnumerable<AccountDetailDto> accountDetailDtos, IEnumerable<ErrorDto> legalPersonValidationBlockingErrors)
        {
            var blockingErrors = new List<ErrorDto>();
            foreach (var accountDetailDto in accountDetailDtos)
            {
                if (accountDetailDto.Type == ExportOrderType.IncomingFromFranchiseesClient)
                {
                    // Для входящей рег. рекламы из ДГПП подтягиваем доп. данные:
                    var accountDetailForIncomingFromClient = GetAccountDetailForIncomingFromClient(accountDetailDto.OrderId);

                    if (accountDetailForIncomingFromClient != null)
                    {
                        accountDetailDto.BranchOfficeOrganizationUnitSyncCode1C = accountDetailForIncomingFromClient.BranchOfficeOrganizationUnitSyncCode1C;

                        if (accountDetailForIncomingFromClient.AccountInfo != null)
                        {
                            accountDetailDto.AccountCode = accountDetailForIncomingFromClient.AccountInfo.Id;
                            accountDetailDto.ProfileCode = accountDetailForIncomingFromClient.AccountInfo.LegalPersonProfileId;
                            accountDetailDto.LegalPersonSyncCode1C = accountDetailForIncomingFromClient.AccountInfo.LegalPersonSyncCode1C;
                        }
                        else
                        {
                            blockingErrors.Add(new ErrorDto
                            {
                                ErrorMessage = string.Format(BLResources.AccountIsMissing,
                                                             accountDetailForIncomingFromClient.ExecutingBranchOfficeName + "(" + accountDetailForIncomingFromClient.ExecutingBranchOfficeId + ")",
                                                             accountDetailForIncomingFromClient.DestBranchOfficeName + "(" + accountDetailForIncomingFromClient.DestBranchOfficeId + ")",
                                                             accountDetailForIncomingFromClient.OrderNumber),
                                LegalPersonId = accountDetailDto.LegalPerson.Id,
                            });
                        }
                    }
                }

                if (!accountDetailDto.OrderHasPositionsWithPlannedProvision // для операций списания по заказам, в которых есть позиции с негарантированным размещением проверку не выполняем (см. https://jira.2gis.ru/browse/ERM-4102)
                        && Math.Abs(accountDetailDto.DebitAccountDetailAmount - accountDetailDto.PlatformDistributions.Sum(y => y.Value)) >= Accuracy)
                {
                    blockingErrors.Add(new ErrorDto
                    {
                        ErrorMessage = string.Format(BLResources.DifferenceBetweenAccountDetailAndPlatformDistibutionsIsTooBig, accountDetailDto.OrderNumber),
                        LegalPersonId = accountDetailDto.LegalPerson.Id,
                    });
                }
            }

            return legalPersonValidationBlockingErrors.Concat(blockingErrors).ToList();
        }

        private AccountDetailForIncomingFromClient GetAccountDetailForIncomingFromClient(long orderId)
        {
            var activeAccountsQuery = _finder.Find<Account>(x => x.IsActive && !x.IsDeleted);
            return (from Order order in _finder.Find<Order>(x => x.Id == orderId)
                    let destBranchOfficeOrgUnit = order.DestOrganizationUnit.BranchOfficeOrganizationUnits
                                                       .FirstOrDefault(z => z.IsPrimaryForRegionalSales)
                    let destBranchOffice = destBranchOfficeOrgUnit.BranchOffice
                    let executingBranchOfficeOrgUnit = order.BranchOfficeOrganizationUnit
                    let executingBranchOffice = executingBranchOfficeOrgUnit.BranchOffice
                    let executingBranchOfficeInn = order.BranchOfficeOrganizationUnit.BranchOffice.Inn
                    let executingBranchOfficeKpp = order.BranchOfficeOrganizationUnit.Kpp
                    let executingBranchOfficeName = executingBranchOfficeOrgUnit.BranchOffice.Name
                    let destBranchOfficeName = destBranchOfficeOrgUnit.BranchOffice.Name
                    let orderNumber = order.Number
                    let executingBranchOfficeId = executingBranchOfficeOrgUnit.BranchOffice.Id
                    let destBranchOfficeId = destBranchOfficeOrgUnit.BranchOffice.Id
                    select new AccountDetailForIncomingFromClient
                    {
                        BranchOfficeOrganizationUnitSyncCode1C = destBranchOfficeOrgUnit.SyncCode1C,
                        AccountInfo = activeAccountsQuery.Where(x => x.IsActive && !x.IsDeleted)
                                                         .Where(x => x.BranchOfficeOrganizationUnit.BranchOffice.Inn == destBranchOffice.Inn &&
                                                                     x.BranchOfficeOrganizationUnit.Kpp == destBranchOfficeOrgUnit.Kpp &&
                                                                     x.BranchOfficeOrganizationUnit.OrganizationUnitId == order.DestOrganizationUnitId)
                                                         .Where(x => (x.LegalPerson.LegalPersonTypeEnum == LegalPersonType.Businessman)
                                                                         ? x.LegalPerson.Inn == executingBranchOfficeInn &&
                                                                           x.LegalPerson.Kpp == null
                                                                         : x.LegalPerson.Inn == executingBranchOfficeInn &&
                                                                           x.LegalPerson.Kpp == executingBranchOfficeKpp)
                                                         .Select(x => new AccountInfo
                                                         {
                                                             Id = x.Id,
                                                             LegalPersonProfileId = x.LegalPerson.LegalPersonProfiles
                                                                                     .Where(p => !p.IsDeleted && p.IsMainProfile)
                                                                                     .Select(p => p.Id)
                                                                                     .FirstOrDefault(),
                                                             LegalPersonSyncCode1C = x.LegalPesonSyncCode1C,
                                                         })
                                                         .FirstOrDefault(),
                        ExecutingBranchOfficeName = executingBranchOfficeName,
                        OrderNumber = orderNumber,
                        DestBranchOfficeName = destBranchOfficeName,
                        ExecutingBranchOfficeId = executingBranchOfficeId,
                        DestBranchOfficeId = destBranchOfficeId
                    })
                .FirstOrDefault();
        }

        #region Helper Types

        private sealed class AccountDetailDto
        {
            public bool OrderHasPositionsWithPlannedProvision { get; set; }
            public long OrderId { get; set; }
            public LegalPerson LegalPerson { get; set; }
            public string BranchOfficeOrganizationUnitSyncCode1C { get; set; }
            public string LegalPersonSyncCode1C { get; set; }
            public OrderType OrderType { get; set; }
            public string OrderNumber { get; set; }
            public DateTime OrderSignupDateUtc { get; set; }
            public string ElectronicMedia { get; set; }
            public decimal DebitAccountDetailAmount { get; set; }
            public Dictionary<PlatformEnum, decimal> PlatformDistributions { get; set; }
            public ExportOrderType Type { get; set; }

            public long AccountCode { get; set; }

            public string ClientOrderNumber { get; set; }

            public long ProfileCode { get; set; }
        }

        private sealed class AccountDetailForIncomingFromClient
        {
            public string BranchOfficeOrganizationUnitSyncCode1C { get; set; }
            public AccountInfo AccountInfo { get; set; }

            public string ExecutingBranchOfficeName { get; set; }
            public string DestBranchOfficeName { get; set; }
            public string OrderNumber { get; set; }
            public long ExecutingBranchOfficeId { get; set; }
            public long DestBranchOfficeId { get; set; }
        }

        private sealed class AccountInfo
        {
            public long Id { get; set; }
            public long LegalPersonProfileId { get; set; }
            public string LegalPersonSyncCode1C { get; set; }
        }

        #endregion
    }
}