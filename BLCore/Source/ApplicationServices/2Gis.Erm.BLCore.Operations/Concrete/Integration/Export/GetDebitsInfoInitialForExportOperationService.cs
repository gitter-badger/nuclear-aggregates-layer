using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils.Data;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    public sealed class GetDebitsInfoInitialForExportOperationService : IGetDebitsInfoInitialForExportOperationService
    {
        private const decimal Accuracy = 1M;

        private readonly IOrderReadModel _orderReadModel;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IValidateLegalPersonsForExportOperationService _validateLegalPersonsForExportOperationService;

        public GetDebitsInfoInitialForExportOperationService(
            IOrderReadModel orderReadModel,
            IAccountReadModel accountReadModel,
            IValidateLegalPersonsForExportOperationService validateLegalPersonsForExportOperationService)
        {
            _orderReadModel = orderReadModel;
            _accountReadModel = accountReadModel;
            _validateLegalPersonsForExportOperationService = validateLegalPersonsForExportOperationService;
        }

        public IEnumerable<DebitsInfoInitialDto> Get(DateTime startPeriodDate, DateTime endPeriodDate, IEnumerable<long> organizationUnitIds)
        {
            var accountDetailDtosByOrganizationUnit = _accountReadModel.GetAccountDetailsForExportTo1C(organizationUnitIds, startPeriodDate, endPeriodDate);
            var allAccountDetailDtos = accountDetailDtosByOrganizationUnit.SelectMany(x => x.Value).ToArray();

            var dtosToValidate = allAccountDetailDtos.DistinctBy(x => x.LegalPersonId).ToArray();
            var legalPersonsErrors = _validateLegalPersonsForExportOperationService
                .Validate(dtosToValidate.Select(x => new ValidateLegalPersonRequestItem
                                                         {
                                                             LegalPersonId = x.LegalPersonId,
                                                             SyncCode1C = x.LegalPersonSyncCode1C
                                                         }));

            var orderIds = allAccountDetailDtos.Select(x => x.OrderId).Distinct().ToArray();
            var distributions = _orderReadModel.GetOrderPlatformDistributions(orderIds, startPeriodDate, endPeriodDate);
            foreach (var accountDetailDto in allAccountDetailDtos)
            {
                accountDetailDto.PlatformDistributions = distributions[accountDetailDto.OrderId];
            }

            var platformDistibutionErrors = ValidatePlatformDistibutionsAccuracy(allAccountDetailDtos);
            return accountDetailDtosByOrganizationUnit.Select(x => ConvertToDebitsInfoInitialDto(startPeriodDate, endPeriodDate, x.Value)).ToArray();
        }

        private DebitsInfoInitialDto ConvertToDebitsInfoInitialDto(DateTime startPeriodDate,
                                                                   DateTime endPeriodDate,
                                                                   IEnumerable<AccountDetailForExportDto> accountDetailDtos)
        {
            var debits =
                accountDetailDtos.Select(x => new DebitDto
                                                  {
                                                      OrderCode = x.OrderId,
                                                      AccountCode = x.AccountCode,
                                                      ProfileCode = x.ProfileCode,
                                                      Amount = x.DebitAccountDetailAmount,
                                                      SignupDate = x.OrderSignupDateUtc,
                                                      ClientOrderNumber = x.ClientOrderNumber,
                                                      OrderType = (int)x.OrderType,
                                                      OrderNumber = x.OrderNumber,
                                                      MediaInfo = x.ElectronicMedia,
                                                      LegalEntityBranchCode1C = x.BranchOfficeOrganizationUnitSyncCode1C,
                                                      Type = DebitDto.DebitType.Client,
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
                                                  }).ToArray();

            return new DebitsInfoInitialDto
            {
                OrganizationUnitCode = accountDetailDtos.Select(x => x.OrganizationUnitSyncCode1C).Distinct().Single(),
                StartDate = startPeriodDate,
                EndDate = endPeriodDate,
                ClientDebitTotalAmount = debits.Sum(x => x.Amount),
                Debits = debits
            };
        }

        private IEnumerable<ErrorDto> ValidatePlatformDistibutionsAccuracy(IEnumerable<AccountDetailForExportDto> accountDetailDtos)
        {
            // для операций списания по заказам, в которых есть позиции с негарантированным размещением проверку не выполняем (см. https://jira.2gis.ru/browse/ERM-4102)
            return (from accountDetailDto in accountDetailDtos.Where(x => !x.OrderHasPositionsWithPlannedProvision)
                    where Math.Abs(accountDetailDto.DebitAccountDetailAmount - accountDetailDto.PlatformDistributions.Sum(y => y.Value)) >= Accuracy
                    select new ErrorDto
                    {
                        ErrorMessage = string.Format(BLResources.DifferenceBetweenAccountDetailAndPlatformDistibutionsIsTooBig, accountDetailDto.OrderNumber),
                        LegalPersonId = accountDetailDto.LegalPersonId,
                        IsBlockingError = true
                    }).ToArray();
        }
    }
}