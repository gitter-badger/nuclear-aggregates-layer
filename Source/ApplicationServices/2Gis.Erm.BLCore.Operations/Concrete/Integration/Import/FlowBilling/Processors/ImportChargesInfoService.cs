using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Withdrawals.Dto;
using DoubleGis.Erm.BLCore.Aggregates.Withdrawals.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Withdrawals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus.Import;
using DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus.Import.FlowBilling;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge;

using ChargeBusDto = DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing.ChargeDto;
using ChargeDto = DoubleGis.Erm.BLCore.Aggregates.Withdrawals.Operations.ChargeDto;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowBilling.Processors
{
    public class ImportChargesInfoService : IImportChargesInfoService
    {
        private readonly IBulkCreateChargesAggregateService _bulkCreateChargesAggregateService;
        private readonly ICreateChargesHistoryAggregateService _createChargesHistoryAggregateService;
        private readonly IDeleteChargesForPeriodAndProjectOperationService _deleteChargesService;
        private readonly ICommonLog _logger;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IWithdrawalInfoReadModel _withdrawalInfoReadModel;

        public ImportChargesInfoService(IBulkCreateChargesAggregateService bulkCreateChargesAggregateService,
                                        ICreateChargesHistoryAggregateService createChargesHistoryAggregateService,
                                        IDeleteChargesForPeriodAndProjectOperationService deleteChargesService,
                                        ICommonLog logger,
                                        IOrderReadModel orderReadModel,
                                        IOperationScopeFactory scopeFactory,
                                        IWithdrawalInfoReadModel withdrawalInfoReadModel)
        {
            _bulkCreateChargesAggregateService = bulkCreateChargesAggregateService;
            _createChargesHistoryAggregateService = createChargesHistoryAggregateService;
            _deleteChargesService = deleteChargesService;
            _logger = logger;
            _orderReadModel = orderReadModel;
            _scopeFactory = scopeFactory;
            _withdrawalInfoReadModel = withdrawalInfoReadModel;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            foreach (var dto in dtos)
            {
                try
                {
                    ProcessChargesInfo((ChargesInfoServiceBusDto)dto);
                }
                catch (Exception e)
                {
                    _logger.ErrorEx(e, e.Message);
                    using (var transaction = new TransactionScope(TransactionScopeOption.Suppress, DefaultTransactionOptions.Default))
                    {
                        LogImportStatus((ChargesInfoServiceBusDto)dto, ChargesHistoryStatus.Error, e.Message);
                        transaction.Complete();
                    }

                    throw new NonBlockingImportErrorException("An error occurred during charges import", e);
                }
            }
        }

        private void ProcessChargesInfo(ChargesInfoServiceBusDto chargesInfo)
        {
            string error;
            if (!_withdrawalInfoReadModel.CanCreateCharges(chargesInfo.BranchCode, new TimePeriod(chargesInfo.StartDate, chargesInfo.EndDate), out error))
            {
                throw new CannotCreateChargesException(error);
            }

            var orderPositionChargesInfoToDtoMap = chargesInfo.Charges.ToDictionary(x => new OrderPositionChargeInfo
                {
                    CategoryId = x.RubricCode,
                    FirmId = x.FirmCode,
                    PositionId = x.NomenclatureElementCode
                });

            using (var scope = _scopeFactory.CreateNonCoupled<ImportChargesInfoIdentity>())
            {
                var timePeriod = new TimePeriod(chargesInfo.StartDate, chargesInfo.EndDate);

                IReadOnlyDictionary<OrderPositionChargeInfo, long> acquiredOrderPositions;
                string report;
                if (!_orderReadModel.TryAcquireOrderPositions(chargesInfo.BranchCode,
                                                              timePeriod,
                                                              orderPositionChargesInfoToDtoMap.Keys.ToArray(),
                                                              out acquiredOrderPositions,
                                                              out report))
                {
                    // FIXME {a.tukaev, 15.05.2014}: Та же тема, есть в нескольких местах. Лучше выкидывать проблемноориентированные (о_О) исключения, а дальше их оборачивать для логирования/UI
                    // DONE {d.ivanov, 20.05.2014}: +1
                    throw new CannotAcquireOrderPositionsForChargesException(report);
                }

                _deleteChargesService.Delete(chargesInfo.BranchCode, timePeriod, chargesInfo.SessionId);

                var chargesToCreate = CreateCharges(orderPositionChargesInfoToDtoMap, acquiredOrderPositions);
                _bulkCreateChargesAggregateService.Create(chargesInfo.BranchCode,
                                                          chargesInfo.StartDate,
                                                          chargesInfo.EndDate,
                                                          chargesToCreate,
                                                          chargesInfo.SessionId);

                LogImportStatus(chargesInfo, ChargesHistoryStatus.Succeeded, null);
                scope.Complete();
            }
        }

        private static List<ChargeDto> CreateCharges(Dictionary<OrderPositionChargeInfo, ChargeBusDto> dtoToBusObjectsMap,
                                                     IReadOnlyDictionary<OrderPositionChargeInfo, long> acquiredOrderPositions)
        {
            var chargesToCreate = new List<ChargeDto>();
            foreach (var item in dtoToBusObjectsMap)
            {
                var chargeInfo = item.Value;
                var orderPositionId = acquiredOrderPositions[item.Key];

                var dto = new ChargeDto
                    {
                        OrderPositionId = orderPositionId,
                        PositionId = chargeInfo.NomenclatureElementToChargeCode,
                    };

                chargesToCreate.Add(dto);
            }

            return chargesToCreate;
        }

        private void LogImportStatus(ChargesInfoServiceBusDto chargesInfo, ChargesHistoryStatus status, string comment)
        {
            var chargesHistoryDto = new ChargesHistoryDto
                {
                    ProjectId = chargesInfo.BranchCode,
                    PeriodStartDate = chargesInfo.StartDate,
                    PeriodEndDate = chargesInfo.EndDate,
                    Message = chargesInfo.Content.ToString(SaveOptions.None),
                    Status = status,
                    SessionId = chargesInfo.SessionId,
                    Comment = comment
                };

            _createChargesHistoryAggregateService.Create(chargesHistoryDto);
        }
    }
}