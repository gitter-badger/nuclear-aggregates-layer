using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    public sealed class GetDebitsInfoInitialForExportOperationService : IGetDebitsInfoInitialForExportOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IAccountReadModel _accountReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public GetDebitsInfoInitialForExportOperationService(IOrderReadModel orderReadModel, IAccountReadModel accountReadModel, IOperationScopeFactory operationScopeFactory)
        {
            _orderReadModel = orderReadModel;
            _accountReadModel = accountReadModel;
            _operationScopeFactory = operationScopeFactory;
        }

        public IDictionary<long, DebitsInfoInitialDto> Get(DateTime startPeriodDate, DateTime endPeriodDate, IEnumerable<long> organizationUnitIds)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetDebitsInfoInitialForExportIdentity>())
            {
                var accountDetailDtosByOrganizationUnit = _accountReadModel.GetAccountDetailsForExportTo1C(organizationUnitIds, startPeriodDate, endPeriodDate);
                var allAccountDetailDtos = accountDetailDtosByOrganizationUnit.SelectMany(x => x.Value).ToArray();

                var distributions = new Dictionary<long, Dictionary<PlatformEnum, decimal>>();
                const int OrdersChunkSize = 300;
                var processedOrdersCount = 0;
                var orderIds = allAccountDetailDtos.Select(x => x.OrderId).Distinct().ToArray();
                var ordersCount = orderIds.Count();
                while (processedOrdersCount < ordersCount)
                {
                    var distributionsBunch = _orderReadModel.GetOrderPlatformDistributions(orderIds.Skip(processedOrdersCount).Take(OrdersChunkSize), startPeriodDate, endPeriodDate);
                    foreach (var item in distributionsBunch)
                    {
                        distributions.Add(item.Key, item.Value);
                    }

                    processedOrdersCount += OrdersChunkSize;
                }

                foreach (var accountDetailDto in allAccountDetailDtos)
                {
                    accountDetailDto.PlatformDistributions = distributions[accountDetailDto.OrderId];
                }

                scope.Complete();

                return accountDetailDtosByOrganizationUnit.ToDictionary(x => x.Key, y => ConvertToDebitsInfoInitialDto(startPeriodDate, endPeriodDate, y.Value));
            }
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
    }
}