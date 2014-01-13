using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Common.Xml;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using SaveOptions = System.Xml.Linq.SaveOptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    public sealed class ExportAccountDetailsToServiceBusForBranchHandler : RequestHandler<ExportAccountDetailsToServiceBusForBranchRequest, IntegrationResponse>
    {
        private static readonly Encoding CyrillicEncoding = Encoding.GetEncoding(1251);

        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IOrderRepository _orderRepository;

        public ExportAccountDetailsToServiceBusForBranchHandler(
            IFinder finder, 
            ISubRequestProcessor subRequestProcessor, 
            IClientProxyFactory clientProxyFactory, 
            IOrderRepository orderRepository)
        {
            _finder = finder;
            _subRequestProcessor = subRequestProcessor;
            _clientProxyFactory = clientProxyFactory;
            _orderRepository = orderRepository;
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
            var branchInfo = _finder.FindAll<OrganizationUnit>().Where(x => x.Id == request.OrganizationUnitId && x.ErmLaunchDate != null).Select(x => new
            {
                PrimaryBranchOfficeOrganizationUnit = x.BranchOfficeOrganizationUnits.FirstOrDefault(y => y.IsPrimary),
                OrganizationUnitSyncCode1c = x.SyncCode1C
            })
            .FirstOrDefault(x => x.PrimaryBranchOfficeOrganizationUnit != null && x.PrimaryBranchOfficeOrganizationUnit.BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Branch);

            if (branchInfo == null)
            {
                throw new NotificationException(BLResources.SelectedOrganizationUnitIsNotBranchOrNotMovedToERM);
            }

            var accountDetail = _finder.FindAll<AccountDetail>();

            // старая логика работает только для заказов размещающихся до 1 апреля
            var firstApril = new DateTime(2013, 4, 1);
            const string RussianhOf = "оф";
            const string EnglishOf = "oф";

            var nonAllowedPlatforms = new[] { PlatformEnum.Api, PlatformEnum.Online };

            var all = (from @lock in _finder.FindAll<Lock>()
                       let type = @lock.Order.SourceOrganizationUnitId == request.OrganizationUnitId
                                ? ExportOrderType.LocalAndOutgoing
                                : @lock.Order.DestOrganizationUnitId == request.OrganizationUnitId && @lock.Order.SourceOrganizationUnit.BranchOfficeOrganizationUnits
                                                                                                           .FirstOrDefault(x => x.IsPrimary)
                                                                                                           .BranchOffice.ContributionTypeId == (int)ContributionTypeEnum.Franchisees
                                            ? (@lock.Order.Number.ToLower().StartsWith(RussianhOf) || @lock.Order.Number.ToLower().StartsWith(EnglishOf)
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
                       select new 
                           {
                               Lock = @lock, 
                               Type = type
                           }).Select(x =>
                           x.Type == ExportOrderType.LocalAndOutgoing || x.Type == ExportOrderType.IncomingFromFranchiseesDgpp
                               ? new AccountDetailDto
                                     {
                                         BranchOfficeOrganizationUnitSyncCode1C = x.Lock.Account.BranchOfficeOrganizationUnit.SyncCode1C, 

                                         AccountCode = x.Lock.Account.Id, 
                                         ProfileCode = x.Lock.Order.LegalPersonProfileId != null ? x.Lock.Order.LegalPersonProfile.Id
                                                                                                 : x.Lock.Account
                                                                                                         .LegalPerson
                                                                                                         .LegalPersonProfiles.Where(p => !p.IsDeleted && p.IsMainProfile)
                                                                                                                             .Select(p => p.Id)
                                                                                                                             .FirstOrDefault(), 
                                         ClientOrderNumber = null, 
                                         LegalPersonSyncCode1C = x.Lock.Account.LegalPesonSyncCode1C, 
                                         OrderNumber = x.Lock.Order.Number, 
                                         OrderSignupDateUtc = x.Lock.Order.SignupDate, 
                                         DebitAccountDetailAmount = accountDetail.Where(y => y.Id == x.Lock.DebitAccountDetailId)
                                                                                 .Select(y => y.Amount)
                                                                                 .FirstOrDefault(), 
                                         ElectronicMedia = x.Lock.Order.DestOrganizationUnit.ElectronicMedia,  
                                         OrderId = x.Lock.OrderId, 
                                         LegalPerson = x.Lock.Account.LegalPerson, 
                                         Type = x.Type, 
                                     }

                               // ExportOrderType.IncomingFromFranchiseesClient
                               : new AccountDetailDto
                                     {
                                         // Поля заполняются ниже из extension 
                                         BranchOfficeOrganizationUnitSyncCode1C = string.Empty, 
                                         AccountCode = 0, 
                                         ProfileCode = 0, 

                                         ClientOrderNumber = x.Lock.Order.Number, 
                                         LegalPersonSyncCode1C = x.Lock.Account.LegalPesonSyncCode1C, 
                                         OrderNumber = x.Lock.Order.RegionalNumber, 
                                         OrderSignupDateUtc = x.Lock.Order.SignupDate, 
                                         DebitAccountDetailAmount = accountDetail.Where(y => y.Id == x.Lock.DebitAccountDetailId)
                                                                                 .Select(y => y.Amount)
                                                                                 .FirstOrDefault(), 
                                         ElectronicMedia = x.Lock.Order.DestOrganizationUnit.ElectronicMedia, 
                                         OrderId = x.Lock.OrderId, 
                                         LegalPerson = x.Lock.Account.LegalPerson, 
                                         Type = x.Type,
                                     }).Where(x => x.DebitAccountDetailAmount > 0).ToArray();

            var orderIds = all.Select(x => x.OrderId).Distinct().ToArray();

            var distributions = _orderRepository.GetOrderPlatformDistributions(orderIds, request.StartPeriodDate, request.EndPeriodDate);

            foreach (var accountDetailDto in all)
            {
                accountDetailDto.PlatformDistributions = distributions[accountDetailDto.OrderId];
            }

            var blockingErrors = new List<ErrorDto>();

            const decimal Accuracy = 1M;

            foreach (var accountDetailDto in all)
            {
                if (accountDetailDto.Type == ExportOrderType.IncomingFromFranchiseesClient)
                {
                    var dto = accountDetailDto;
                    var accountQuery = _finder.Find<Account>(x => x.IsActive && !x.IsDeleted);

                    // Для входящей рег. рекламы из ДГПП подтягиваем доп. данные:
                    var extension = (from Order order in _finder.Find<Order>(x => x.Id == dto.OrderId)
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
                                     select new AccountDetailForIncomingFromClient
                                         {
                                             BranchOfficeOrganizationUnitSyncCode1C = destBranchOfficeOrgUnit.SyncCode1C,
                                             AccountInfo = accountQuery
                                         .Where(x => x.IsActive && !x.IsDeleted)
                                         .Where(x => x.BranchOfficeOrganizationUnit.BranchOffice.Inn == destBranchOffice.Inn &&
                                                     x.BranchOfficeOrganizationUnit.Kpp == destBranchOfficeOrgUnit.Kpp &&
                                                     x.BranchOfficeOrganizationUnit.OrganizationUnitId == order.DestOrganizationUnitId)
                                         .Where(x => (x.LegalPerson.LegalPersonTypeEnum == (int)LegalPersonType.Businessman)
                                                         ? x.LegalPerson.Inn == executingBranchOfficeInn && x.LegalPerson.Kpp == null
                                                         : x.LegalPerson.Inn == executingBranchOfficeInn && x.LegalPerson.Kpp == executingBranchOfficeKpp)
                                         .Select(x => new AccountInfo
                                             {
                                                 Id = x.Id,
                                                 LegalPersonProfileId = x.LegalPerson.LegalPersonProfiles.Where(p => !p.IsDeleted && p.IsMainProfile)
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
                                         }).FirstOrDefault();

                    if (extension != null)
                    {
                        accountDetailDto.BranchOfficeOrganizationUnitSyncCode1C = extension.BranchOfficeOrganizationUnitSyncCode1C;

                        if (extension.AccountInfo != null)
                        {
                            accountDetailDto.AccountCode = extension.AccountInfo.Id;
                            accountDetailDto.ProfileCode = extension.AccountInfo.LegalPersonProfileId;
                            accountDetailDto.LegalPersonSyncCode1C = extension.AccountInfo.LegalPersonSyncCode1C;
                        }
                        else
                        {
                            blockingErrors.Add(new ErrorDto
                                {
                                    ErrorMessage = string.Format(BLResources.AccountIsMissing,
                                                                 extension.ExecutingBranchOfficeName + "(" + extension.ExecutingBranchOfficeId + ")",
                                                                 extension.DestBranchOfficeName + "(" + extension.DestBranchOfficeId + ")",
                                                                 extension.OrderNumber),
                                    LegalPersonId = accountDetailDto.LegalPerson.Id,
                                });
                        }
                    }
                }

                if (Math.Abs(accountDetailDto.DebitAccountDetailAmount - accountDetailDto.PlatformDistributions.Sum(y => y.Value)) >= Accuracy)
                {
                    blockingErrors.Add(new ErrorDto
                        {
                            ErrorMessage = string.Format(BLResources.DifferenceBetweenAccountDetailAndPlatformDistibutionsIsTooBig, accountDetailDto.OrderNumber),
                            LegalPersonId = accountDetailDto.LegalPerson.Id,
                        });
                }
            }

            var dtosToValidate = all.Where(x => x.LegalPersonSyncCode1C != null).DistinctBy(x => x.LegalPersonSyncCode1C).ToArray();

            var debitsInfo = new DebitsInfoDto
                {
                    StartDate = request.StartPeriodDate,
                    EndDate = request.EndPeriodDate,
                    OrganizationUnitCode = branchInfo.OrganizationUnitSyncCode1c,
                    Debits = all.Select(x => new DebitDto
                        {
                            AccountCode = x.AccountCode,
                            ProfileCode = x.ProfileCode,
                            Amount = GetAccountDetailAmount(x),
                            SignupDate = x.OrderSignupDateUtc,
                            ClientOrderNumber = x.ClientOrderNumber,
                            OrderNumber = x.OrderNumber,
                            MediaInfo = x.ElectronicMedia,
                            LegalEntityBranchCode1C = x.BranchOfficeOrganizationUnitSyncCode1C,
                            Type =
                                x.Type == ExportOrderType.LocalAndOutgoing || x.Type == ExportOrderType.IncomingFromFranchiseesDgpp
                                    ? DebitDto.DebitType.Client
                                    : DebitDto.DebitType.Regional,
                            PlatformDistributions = new[]
                                {
                                    new PlatformDistribution
                                        {
                                            PlatformCode = PlatformEnum.Desktop,
                                            Amount = x.PlatformDistributions.ContainsKey(PlatformEnum.Desktop) ? x.PlatformDistributions[PlatformEnum.Desktop] : 0
                                        },
                                    new PlatformDistribution
                                        {
                                            PlatformCode = PlatformEnum.Mobile,
                                            Amount = x.PlatformDistributions.ContainsKey(PlatformEnum.Mobile) ? x.PlatformDistributions[PlatformEnum.Mobile] : 0
                                        },
                                    new PlatformDistribution
                                        {
                                            PlatformCode = PlatformEnum.Api,
                                            Amount = x.PlatformDistributions.ContainsKey(PlatformEnum.Api) ? x.PlatformDistributions[PlatformEnum.Api] : 0
                                        },
                                    new PlatformDistribution
                                        {
                                            PlatformCode = PlatformEnum.Online,
                                            Amount = x.PlatformDistributions.ContainsKey(PlatformEnum.Online) ? x.PlatformDistributions[PlatformEnum.Online] : 0
                                        }
                                }
                        }).ToArray()
                };

            var validateResponse = ValidateLegalPersons(dtosToValidate);

            var streamDictionary = new Dictionary<string, Stream>();
            var errorsFileName = "ExportErrors_" + DateTime.Today.ToShortDateString() + ".csv";

            var modiResponse = AccountDetailsFrom1CHelper.ExportRegionalAccountDetailsToServiceBus(
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

            if (!debitsInfo.Debits.Any() && modiResponse.ProcessedWithoutErrors + modiResponse.BlockingErrorsAmount + modiResponse.NonBlockingErrorsAmount == 0)
            {
                throw new NotificationException(string.Format(BLResources.NoDebitsForSpecifiedPeriod, request.StartPeriodDate, request.EndPeriodDate));
            }

            var errorContent = GetErrorsDataTable(blockingErrors, validateResponse.NonBlockingErrors).ToCsv(';');
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

            var xml = debitsInfo.ToXElement();
            var result = xml.ConcatBy(XElement.Load(new MemoryStream(modiResponse.File.Stream)), "RegionalDebits");
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(result.ToString(SaveOptions.None)));
            streamDictionary.Add("DebitsInfo_" + DateTime.Today.ToShortDateString() + ".xml", stream);

            response.FileName = "Acts.zip";
            response.ContentType = MediaTypeNames.Application.Zip;
            response.Stream = streamDictionary.ZipStreamDictionary();
            response.ProcessedWithoutErrors = debitsInfo.Debits.Length - (blockingErrors.Count + validateResponse.NonBlockingErrors.Count) + modiResponse.ProcessedWithoutErrors;

            return response;
        }

        private static decimal GetAccountDetailAmount(AccountDetailDto accountDetailDto)
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
            return Math.Round(FranchiseesAmountFactor * accountDetailDto.DebitAccountDetailAmount * VatMultiplier, 2, MidpointRounding.ToEven);
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

        #region Helper Types

        private sealed class AccountDetailDto
        {
            public long OrderId { get; set; }
            public LegalPerson LegalPerson { get; set; }
            public string BranchOfficeOrganizationUnitSyncCode1C { get; set; }
            public string LegalPersonSyncCode1C { get; set; }
            public string OrderNumber { get; set; }
            public DateTime OrderSignupDateUtc { get; set; }
            public string ElectronicMedia { get; set; }
            public decimal DebitAccountDetailAmount { get; set; }
            public Dictionary<PlatformEnum, decimal> PlatformDistributions { get; set; }
            public ExportOrderType Type { get; set; }

            public long AccountCode
            {
                get;
                set;
            }

            public string ClientOrderNumber
            {
                get;
                set;
            }

            public long ProfileCode
            {
                get; set;
            }
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