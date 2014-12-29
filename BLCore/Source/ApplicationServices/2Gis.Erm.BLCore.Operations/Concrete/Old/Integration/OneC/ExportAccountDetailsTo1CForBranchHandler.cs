using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class ExportAccountDetailsTo1CForBranchHandler : RequestHandler<ExportAccountDetailsTo1CForBranchRequest, IntegrationResponse>
    {
        private const string NskTimeZoneId = "N. Central Asia Standard Time";
        private static readonly TimeZoneInfo NskTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(NskTimeZoneId);
        private static readonly Encoding CyrillicEncoding = Encoding.GetEncoding(1251);

        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly ICommonLog _logger;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IGlobalizationSettings _globalizationSettings;
        private readonly IUseCaseTuner _useCaseTuner;

        public ExportAccountDetailsTo1CForBranchHandler(
            IFinder finder,
                                                        ISubRequestProcessor subRequestProcessor,
                                                        ISecurityServiceUserIdentifier securityServiceUserIdentifier,
                                                        ICommonLog logger,
                                                        IClientProxyFactory clientProxyFactory,
                                                        IOrderReadModel orderReadModel,
            IGlobalizationSettings globalizationSettings,
            IUseCaseTuner useCaseTuner)
        {
            _finder = finder;
            _subRequestProcessor = subRequestProcessor;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _logger = logger;
            _clientProxyFactory = clientProxyFactory;
            _orderReadModel = orderReadModel;
            _globalizationSettings = globalizationSettings;
            _useCaseTuner = useCaseTuner;
        }

        private enum ExportOrderType
        {
            None = 0,

            LocalAndOutgoing = 1,
            IncomingFromFranchiseesClient = 2,
            IncomingFromFranchiseesDgpp = 3
        }

        protected override IntegrationResponse Handle(ExportAccountDetailsTo1CForBranchRequest request)
        {
            _useCaseTuner.AlterDuration<ExportAccountDetailsTo1CForBranchHandler>();
            var isBranchAndMovedToErm =
                _finder.FindAll<OrganizationUnit>().Where(x => x.Id == request.OrganizationUnitId && x.ErmLaunchDate != null)
                       .Select(x => new
            {
                PrimaryBranchOfficeOrganizationUnit = x.BranchOfficeOrganizationUnits.FirstOrDefault(y => y.IsPrimary),
            })
            .Any(x => x.PrimaryBranchOfficeOrganizationUnit != null && x.PrimaryBranchOfficeOrganizationUnit.BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Branch);

            if (!isBranchAndMovedToErm)
            {
                throw new NotificationException(BLResources.SelectedOrganizationUnitIsNotBranchOrNotMovedToERM);
            }

            var accountDetail = _finder.FindAll<AccountDetail>();

            // старая логика работает только для заказов размещающихся до 1 апреля
            var firstApril = new DateTime(2013, 4, 1);
            const string russianhOf = "оф";
            const string englishOf = "oф";

            var nonAllowedPlatforms = new[] { PlatformEnum.Api, PlatformEnum.Online };

            var allWithoutLegalPerson = (from @lock in _finder.FindAll<Lock>()
                       let type = @lock.Order.SourceOrganizationUnitId == request.OrganizationUnitId
                                ? ExportOrderType.LocalAndOutgoing
                                : @lock.Order.DestOrganizationUnitId == request.OrganizationUnitId
                                  &&
                                        @lock.Order.SourceOrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(x => x.IsPrimary)
                                             .BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Franchisees
                                            ? (@lock.Order.Number.ToLower().StartsWith(russianhOf) || @lock.Order.Number.ToLower().StartsWith(englishOf)
                                ? ExportOrderType.IncomingFromFranchiseesDgpp
                                : (@lock.Order.BeginDistributionDate < firstApril && !nonAllowedPlatforms.Contains((PlatformEnum)@lock.Order.Platform.DgppId))
                                ? ExportOrderType.IncomingFromFranchiseesClient
                                                         : ExportOrderType.None)
                                : ExportOrderType.None
                       where
                           !@lock.IsActive
                           && !@lock.IsDeleted
                           && @lock.PeriodStartDate == request.StartPeriodDate
                           && @lock.PeriodEndDate == request.EndPeriodDate
                           && type != ExportOrderType.None
                       select new LockWithAdditionalInfo
                           {
                               Lock = @lock,
                               Type = type
                           }).Select(x =>
                           x.Type == ExportOrderType.LocalAndOutgoing || x.Type == ExportOrderType.IncomingFromFranchiseesDgpp
                               ? new {
                                   AccountDetailDto = new AccountDetailDto
                                     {
                                         OrderHasPositionsWithPlannedProvision =
                                                x.Lock.Order.OrderPositions.Any(op => op.IsActive
                                                    && !op.IsDeleted
                                                    && op.PricePosition.Position.SalesModel == SalesModel.PlannedProvision),

                                         BargainTypeSyncCode1C = x.Lock.Account.BranchOfficeOrganizationUnit.BranchOffice.BargainType.SyncCode1C,
                                         BranchOfficeOrganizationUnitSyncCode1C = x.Lock.Account.BranchOfficeOrganizationUnit.SyncCode1C,

                                         LegalPersonSyncCode1C = x.Lock.Account.LegalPesonSyncCode1C,
                                         OrderNumber = x.Lock.Order.Number,
                                         OrderSignupDateUtc = x.Lock.Order.SignupDate,
                                         DebitAccountDetailAmount =
                                             accountDetail.Where(y => y.Id == x.Lock.DebitAccountDetailId && y.Amount > 0)
                                                          .Select(y => (decimal?)y.Amount)
                                                          .FirstOrDefault(),
                                         ElectronicMedia = x.Lock.Order.DestOrganizationUnit.ElectronicMedia,
                                         PlatformId = x.Lock.Order.Platform.DgppId,
                                         OrderId = x.Lock.OrderId,
                                                 Type = x.Type,
                                     },
                                   LegalPersonId = x.Lock.Account.LegalPersonId
                                     }

                               // ExportOrderType.IncomingFromFranchiseesClient
                               : new {
                                   AccountDetailDto = new AccountDetailDto
                                     {
                                         OrderHasPositionsWithPlannedProvision = false,
                                         BargainTypeSyncCode1C = string.Empty,
                                         BranchOfficeOrganizationUnitSyncCode1C = string.Empty,

                                         LegalPersonSyncCode1C = x.Lock.Account.LegalPesonSyncCode1C,
                                         OrderNumber = x.Lock.Order.RegionalNumber,
                                         OrderSignupDateUtc = x.Lock.Order.SignupDate,
                                         DebitAccountDetailAmount =
                                             accountDetail.Where(y => y.Id == x.Lock.DebitAccountDetailId && y.Amount > 0)
                                                          .Select(y => (decimal?)y.Amount)
                                                          .FirstOrDefault(),
                                         ElectronicMedia = x.Lock.Order.DestOrganizationUnit.ElectronicMedia,
                                         PlatformId = x.Lock.Order.Platform.DgppId,
                                         OrderId = x.Lock.OrderId,
                                                 Type = x.Type,
                                     },
                                     LegalPersonId = x.Lock.Account.LegalPersonId
                                     }).ToArray();

            var legalPersons = _finder.FindMany(Specs.Find.ByIds<LegalPerson>(allWithoutLegalPerson.Select(x => x.LegalPersonId)))
                                      .ToDictionary(x => x.Id, x => x);
            var all = allWithoutLegalPerson.Select(x =>
                                                   {
                                                       x.AccountDetailDto.LegalPerson = legalPersons[x.LegalPersonId];
                                                       return x.AccountDetailDto;
                                                   })
                                           .ToArray();

            var orderIds = all.Select(x => x.OrderId).Distinct().ToArray();

            var distributions = _orderReadModel.GetOrderPlatformDistributions(orderIds, request.StartPeriodDate, request.EndPeriodDate);

            foreach (var accountDetailDto in all)
            {
                accountDetailDto.PlatformDistributions = distributions[accountDetailDto.OrderId];
            }

            var blockingErrors = new List<ErrorDto>();

            const decimal Accuracy = 1M;

            foreach (var accountDetailDto in all.Where(x => x.DebitAccountDetailAmount.HasValue))
            {
                AccountDetailForIncomingFromClient extension = null;

                if (accountDetailDto.Type == ExportOrderType.IncomingFromFranchiseesClient)
                {
                    var dto = accountDetailDto;
                    var accountQuery = _finder.Find<Account>(x => x.IsActive && !x.IsDeleted);

                    // Для входящей рег. рекламы из ДГПП подтягиваем доп. данные:
                    extension = (from Order order in _finder.Find<Order>(x => x.Id == dto.OrderId)
                                 let destBranchOfficeOrgUnit =
                                     order.DestOrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(z => z.IsPrimaryForRegionalSales)
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
                                 select
                                     new AccountDetailForIncomingFromClient
                                         {
                                             BargainTypeSyncCode1C = destBranchOffice.BargainType.SyncCode1C,
                                             BranchOfficeOrganizationUnitSyncCode1C = destBranchOfficeOrgUnit.SyncCode1C,
                                             LegalPersonSyncCode1C = accountQuery
                                                .Where(x => x.IsActive && !x.IsDeleted)
                                                .Where(x => x.BranchOfficeOrganizationUnit.BranchOffice.Inn == destBranchOffice.Inn &&
                                                            x.BranchOfficeOrganizationUnit.Kpp == destBranchOfficeOrgUnit.Kpp &&
                                                            x.BranchOfficeOrganizationUnit.OrganizationUnitId == order.DestOrganizationUnitId)
                                                .Where(x => (x.LegalPerson.LegalPersonTypeEnum == LegalPersonType.Businessman)
                                                        ? x.LegalPerson.Inn == executingBranchOfficeInn && x.LegalPerson.Kpp == null
                                                        : x.LegalPerson.Inn == executingBranchOfficeInn && x.LegalPerson.Kpp == executingBranchOfficeKpp)
                                                .Select(x => x.LegalPesonSyncCode1C)
                                                .FirstOrDefault(),
                                             BranchOfficeInn = executingBranchOfficeInn,
                                             BranchOfficeKpp = executingBranchOfficeKpp,
                                             BranchOfficeFullName = executingBranchOffice.Name,
                                             BranchOfficeShortLegalName = executingBranchOfficeOrgUnit.ShortLegalName,
                                             ExecutingBranchOfficeName = executingBranchOfficeName,
                                             OrderNumber = orderNumber,
                                             DestBranchOfficeName = destBranchOfficeName,
                                             ExecutingBranchOfficeId = executingBranchOfficeId,
                                             DestBranchOfficeId = destBranchOfficeId
                                         }).FirstOrDefault();
                }

                if (extension != null)
                {
                    accountDetailDto.LegalPersonSyncCode1C = extension.LegalPersonSyncCode1C;
                    accountDetailDto.BranchOfficeOrganizationUnitSyncCode1C = extension.BranchOfficeOrganizationUnitSyncCode1C;
                    accountDetailDto.BargainTypeSyncCode1C = extension.BargainTypeSyncCode1C;
                    accountDetailDto.Extension = extension;

                    if (string.IsNullOrWhiteSpace(accountDetailDto.LegalPersonSyncCode1C))
                    {
                        blockingErrors.Add(new ErrorDto
                        {
                            ErrorMessage = string.Format(
                                    BLResources.AccountIsMissing,
                                    extension.ExecutingBranchOfficeName + "(" + extension.ExecutingBranchOfficeId + ")",
                                    extension.DestBranchOfficeName + "(" + extension.DestBranchOfficeId + ")",
                                    extension.OrderNumber),
                            LegalPersonId = accountDetailDto.LegalPerson.Id,
                        });
                    }
                }
                else
                {
                    if (!accountDetailDto.OrderHasPositionsWithPlannedProvision // для операций списания по заказам, в которых есть позиции с негарантированным размещением проверку не выполняем (см. https://jira.2gis.ru/browse/ERM-4102)
                        && Math.Abs(accountDetailDto.DebitAccountDetailAmount.Value - accountDetailDto.PlatformDistributions.Sum(y => y.Value)) >= Accuracy)
                    {
                        blockingErrors.Add(new ErrorDto
                            {
                                ErrorMessage = string.Format(
                                    BLResources.DifferenceBetweenAccountDetailAndPlatformDistibutionsIsTooBig,
                                    accountDetailDto.OrderNumber),
                                LegalPersonId = accountDetailDto.LegalPerson.Id,
                            });
                    }   
                }
            }

            var dtosToValidate = all.Where(x => x.LegalPersonSyncCode1C != null).Distinct(new AccountDetailDtoEqualityComparer()).ToArray();
            var validateResponse = ValidateLegalPersons(dtosToValidate);

            var streamDictionary = new Dictionary<string, Stream>();
            var errorsFileName = "ExportErrors_" + DateTime.Today.ToShortDateString() + ".csv";

            var modiResponse = AccountDetailsFrom1CHelper.ExportRegionalAccountDetails(
                                _clientProxyFactory,
                                request.OrganizationUnitId,
                                request.StartPeriodDate,
                                request.EndPeriodDate);

            blockingErrors = validateResponse.BlockingErrors.Concat(blockingErrors).ToList();

            var response = new IntegrationResponse
            {
                BlockingErrorsAmount = blockingErrors.Count + modiResponse.BlockingErrorsAmount,
                NonBlockingErrorsAmount = validateResponse.NonBlockingErrors.Count + modiResponse.NonBlockingErrorsAmount
            };

            var errorContent = GetErrorsDataTable(blockingErrors, validateResponse.NonBlockingErrors).ToCsv(_globalizationSettings.ApplicationCulture.TextInfo.ListSeparator);
            var modiErrorContent = modiResponse.ErrorFile != null ? CyrillicEncoding.GetString(modiResponse.ErrorFile.Stream) : null;

            if (blockingErrors.Any() || modiResponse.BlockingErrorsAmount > 0)
                {
                response.FileName = errorsFileName;
                response.ContentType = MediaTypeNames.Application.Octet;
                response.Stream = new MemoryStream(CyrillicEncoding.GetBytes(errorContent + modiErrorContent));

                return response;
            }

            if (validateResponse.NonBlockingErrors.Any() || modiResponse.NonBlockingErrorsAmount > 0)
            {
                streamDictionary.Add(errorsFileName, new MemoryStream(CyrillicEncoding.GetBytes(errorContent + modiErrorContent)));
            }

            var actsDataTable = PrepareAccountDetailsData(all);
            if (actsDataTable.Rows.Count == 0 && modiResponse.ProcessedWithoutErrors == 0)
            {
                throw new NotificationException(BLResources.DumpAccountDetails_AccountDetailsNotExist);
            }

            var content = actsDataTable.ToCsv(_globalizationSettings.ApplicationCulture.TextInfo.ListSeparator);
            var modiContent = CyrillicEncoding.GetString(modiResponse.File.Stream);

            streamDictionary.Add("Acts.csv", new MemoryStream(CyrillicEncoding.GetBytes(content + modiContent)));

            response.FileName = "Acts.zip";
            response.ContentType = MediaTypeNames.Application.Zip;
            response.Stream = streamDictionary.ZipStreamDictionary();
            response.ProcessedWithoutErrors = actsDataTable.Rows.Count - (blockingErrors.Count + validateResponse.NonBlockingErrors.Count) + modiResponse.ProcessedWithoutErrors;

            return response;
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

        private DataTable PrepareAccountDetailsData(AccountDetailDto[] allAccountDetail)
        {
            // container creation
            var dataTable = new DataTable { Locale = CultureInfo.InvariantCulture };
            for (var i = 0; i < AccountDetailCsvRowBuilder.CsvAttributesCount; i++)
            {
                dataTable.Columns.Add(string.Empty);
            }

            var csvRowBuilderSettings = new AccountDetailCsvRowBuilderSettings
            {
                DateFormatTemplate = "dd.MM.yy",
                FranchiseesAmountFactor = 0.4m,
                VatMultiplier = 1.18m
            };
            var csvRowBuilder = new AccountDetailCsvRowBuilder(_securityServiceUserIdentifier,
                                                               csvRowBuilderSettings);

            var stringBuilder = new StringBuilder(200 + (allAccountDetail.Length * 16));
            stringBuilder.AppendFormat("В файле выгрузки операций списания созданы следующие записи списаний по лицевому счету:");
            stringBuilder.AppendLine();

            foreach (var accountDetailDto in allAccountDetail.Where(x => x.DebitAccountDetailAmount.HasValue))
            {
                AccountDetailForIncomingFromClient extension = null;

                if (accountDetailDto.Type == ExportOrderType.IncomingFromFranchiseesClient)
                {
                    extension = accountDetailDto.Extension;
                }

                var csvRowData = csvRowBuilder.CreateRowData(accountDetailDto, extension);

                dataTable.Rows.Add(csvRowData);
                stringBuilder.AppendFormat("[{0}], [{1}],", accountDetailDto.LegalPersonSyncCode1C, accountDetailDto.BranchOfficeOrganizationUnitSyncCode1C);
                stringBuilder.AppendLine();
            }

            _logger.InfoEx(stringBuilder.ToString());

            return dataTable;
        }

        #region Helper Types

        private sealed class AccountDetailDto
        {
            public bool OrderHasPositionsWithPlannedProvision { get; set; }
            public long OrderId { get; set; }
            public LegalPerson LegalPerson { get; set; }
            public string BargainTypeSyncCode1C { get; set; }
            public string BranchOfficeOrganizationUnitSyncCode1C { get; set; }
            public string LegalPersonSyncCode1C { get; set; }
            public string OrderNumber { get; set; }
            public DateTime OrderSignupDateUtc { get; set; }
            public string ElectronicMedia { get; set; }
            public long? PlatformId { get; set; }
            public decimal? DebitAccountDetailAmount { get; set; }
            public Dictionary<PlatformEnum, decimal> PlatformDistributions { get; set; }
            public ExportOrderType Type { get; set; }
            public AccountDetailForIncomingFromClient Extension { get; set; }
        }

        private sealed class AccountDetailForIncomingFromClient
        {
            public string BargainTypeSyncCode1C { get; set; }
            public string BranchOfficeOrganizationUnitSyncCode1C { get; set; }
            public string LegalPersonSyncCode1C { get; set; }
            public string BranchOfficeFullName { get; set; }
            public string BranchOfficeShortLegalName { get; set; }
            public string BranchOfficeInn { get; set; }
            public string BranchOfficeKpp { get; set; }
            public string ExecutingBranchOfficeName { get; set; }
            public string DestBranchOfficeName { get; set; }
            public string OrderNumber { get; set; }
            public long ExecutingBranchOfficeId { get; set; }
            public long DestBranchOfficeId { get; set; }
        }

        private sealed class AccountDetailCsvRowBuilderSettings
        {
            public string DateFormatTemplate { get; set; }
            public decimal FranchiseesAmountFactor { get; set; }
            public decimal VatMultiplier { get; set; }
        }

        private sealed class AccountDetailCsvRowBuilder
        {
            private readonly IDictionary<long, string> _usersCache = new Dictionary<long, string>();
            private readonly ISecurityServiceUserIdentifier _userIdentifierService;
            private readonly AccountDetailCsvRowBuilderSettings _settings;

            public AccountDetailCsvRowBuilder(
                ISecurityServiceUserIdentifier userIdentifierService, 
                AccountDetailCsvRowBuilderSettings settings)
            {
                _userIdentifierService = userIdentifierService;
                _settings = settings;
            }

            public static int CsvAttributesCount
            {
                get { return 18; } 
            }

            public object[] CreateRowData(AccountDetailDto accountDetailDto, AccountDetailForIncomingFromClient extension)
            {
                if (accountDetailDto.DebitAccountDetailAmount == null)
                {
                    return null;
                }

                var legalPersonOwnerDisplayName = GetUserDisplayName(_userIdentifierService, accountDetailDto.LegalPerson.OwnerCode);

                var isIncFromClient = accountDetailDto.Type == ExportOrderType.IncomingFromFranchiseesClient;

                var inn = isIncFromClient
                                 ? extension.BranchOfficeInn
                                 : GetLegalPersonInnOrPassportSeries(accountDetailDto.LegalPerson);

                var kpp = isIncFromClient
                                 ? extension.BranchOfficeKpp
                                 : GetLegalPersonKppOrPassportNumber(accountDetailDto.LegalPerson);

                return new object[]
                           {
                               ClearText(isIncFromClient ? extension.BargainTypeSyncCode1C : accountDetailDto.BargainTypeSyncCode1C),
                               ClearText(isIncFromClient ? extension.BranchOfficeOrganizationUnitSyncCode1C : accountDetailDto.BranchOfficeOrganizationUnitSyncCode1C),
                               ClearText(isIncFromClient ? extension.LegalPersonSyncCode1C : accountDetailDto.LegalPersonSyncCode1C),
                               ClearText(inn),
                               ClearText(kpp),
                               ClearText(accountDetailDto.OrderNumber),
                        ClearText(GetOrderSignupDate(accountDetailDto.OrderSignupDateUtc)),
                               GetAccountDetailAmount(accountDetailDto),
                               ClearText(isIncFromClient ? extension.BranchOfficeFullName : accountDetailDto.LegalPerson.LegalName),
                               ClearText(isIncFromClient ? extension.BranchOfficeShortLegalName : accountDetailDto.LegalPerson.ShortName),
                               legalPersonOwnerDisplayName,
                               ClearText(accountDetailDto.ElectronicMedia),
                        accountDetailDto.PlatformId,
                        string.Empty,
                        accountDetailDto.Extension != null
                            ? string.Empty
                            : accountDetailDto.PlatformDistributions.ContainsKey(PlatformEnum.Desktop)
                                  ? accountDetailDto.PlatformDistributions[PlatformEnum.Desktop].ToString()
                                  : 0M.ToString(),
                        accountDetailDto.Extension != null
                            ? string.Empty
                            : accountDetailDto.PlatformDistributions.ContainsKey(PlatformEnum.Mobile)
                                  ? accountDetailDto.PlatformDistributions[PlatformEnum.Mobile].ToString()
                                  : 0M.ToString(),
                        accountDetailDto.Extension != null
                            ? string.Empty
                            : accountDetailDto.PlatformDistributions.ContainsKey(PlatformEnum.Online)
                                  ? accountDetailDto.PlatformDistributions[PlatformEnum.Online].ToString()
                                  : 0M.ToString(),
                        accountDetailDto.Extension != null
                            ? string.Empty
                            : accountDetailDto.PlatformDistributions.ContainsKey(PlatformEnum.Api)
                                  ? accountDetailDto.PlatformDistributions[PlatformEnum.Api].ToString()
                                  : 0M.ToString()
                           };
            }

            // TODO: надо пользоваться ToCsvEscaped вместо этой лажи
            private static string ClearText(string arg)
            {
                return !string.IsNullOrEmpty(arg) ? arg.Replace(';', ',') : arg;
            }

            private static string GetLegalPersonInnOrPassportSeries(LegalPerson legalPerson)
            {
                return legalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson
                           ? legalPerson.PassportSeries
                           : legalPerson.Inn;
            }

            private static string GetLegalPersonKppOrPassportNumber(LegalPerson legalPerson)
            {
                return legalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson
                           ? legalPerson.PassportNumber
                           : legalPerson.Kpp;
            }

            private string GetUserDisplayName(ISecurityServiceUserIdentifier userIdentifierService, long code)
            {
                if (_usersCache.ContainsKey(code))
                {
                    return _usersCache[code];
                }

                var displayName = userIdentifierService.GetUserInfo(code).DisplayName;
                _usersCache.Add(code, displayName);
                return displayName;
            }

            private string GetOrderSignupDate(DateTime orderSignupDateUtc)
            {
                var orderSignupDate = orderSignupDateUtc.AddHours(NskTimeZoneInfo.BaseUtcOffset.Hours);

                var orderSignupDateFormatted = !string.IsNullOrEmpty(_settings.DateFormatTemplate)
                           ? orderSignupDate.ToString(_settings.DateFormatTemplate)
                           : orderSignupDate.ToString();
                return orderSignupDateFormatted;
            }

            private decimal GetAccountDetailAmount(AccountDetailDto accountDetailDto/*decimal accountDetailAmount*/)
            {
                if (accountDetailDto.Type == ExportOrderType.LocalAndOutgoing ||
                    accountDetailDto.Type == ExportOrderType.IncomingFromFranchiseesDgpp)
                {
                    // Исходящий или
                    // региональный БЗ из ДГПП (Номер заказа начинается на "ОФ") -> 
                    // Списание берём 100% из соответствующей операции с лицевым счётом.
                    return accountDetailDto.DebitAccountDetailAmount.Value;
                }

                // - клиентский БЗ созданный в ERM либо ДГПП (номер заказа НЕ начинается на "ОФ")
                // -> Списание берём 40% из соответствующей операции с лицевым 
                // счётом и добавляем к этой сумме НДС;
                return Math.Round(_settings.FranchiseesAmountFactor * accountDetailDto.DebitAccountDetailAmount.Value * _settings.VatMultiplier, 2, MidpointRounding.ToEven);
            }
        }

        private sealed class LockWithAdditionalInfo
        {
            public Lock Lock { get; set; }
            public ExportOrderType Type { get; set; }
        }

        private sealed class AccountDetailDtoEqualityComparer : IEqualityComparer<AccountDetailDto>
        {
            bool IEqualityComparer<AccountDetailDto>.Equals(AccountDetailDto x, AccountDetailDto y)
            {
                if (x.LegalPersonSyncCode1C == null)
                {
                    return y.LegalPersonSyncCode1C == null && x.LegalPerson.Id.Equals(y.LegalPerson.Id);
                }

                if (y.LegalPersonSyncCode1C == null)
                {
                    return false;
                }

                return x.LegalPerson.Id.Equals(y.LegalPerson.Id) &&
                                                           x.LegalPersonSyncCode1C.Equals(y.LegalPersonSyncCode1C);
            }

            int IEqualityComparer<AccountDetailDto>.GetHashCode(AccountDetailDto obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                return obj.LegalPerson.Id.GetHashCode() + (obj.LegalPersonSyncCode1C == null
                                                               ? 0
                                                               : obj.LegalPersonSyncCode1C.GetHashCode());
            }
        }

        #endregion
    }
}