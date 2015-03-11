using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus.Import;
using DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus.Import.FlowBilling;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge;

using Nuclear.Tracing.API;
namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowBilling.Processors
{
    public class ImportChargesInfoService : IImportChargesInfoService
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IChargeBulkCreateAggregateService _chargeBulkCreateAggregateService;
        private readonly IChargeCreateHistoryAggregateService _chargeCreateHistoryAggregateService;
        private readonly IDeleteChargesForPeriodAndProjectOperationService _deleteChargesService;
        private readonly ITracer _tracer;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IChargeReadModel _chargeReadModel;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IProjectService _projectService;

        public ImportChargesInfoService(IAccountReadModel accountReadModel,
                                        IChargeBulkCreateAggregateService chargeBulkCreateAggregateService,
                                        IChargeCreateHistoryAggregateService chargeCreateHistoryAggregateService,
                                        IDeleteChargesForPeriodAndProjectOperationService deleteChargesService,
                                        ITracer tracer,
                                        IOperationScopeFactory scopeFactory,
                                        IChargeReadModel chargeReadModel,
                                        IOrderReadModel orderReadModel,
                                        IProjectService projectService)
        {
            _accountReadModel = accountReadModel;
            _chargeBulkCreateAggregateService = chargeBulkCreateAggregateService;
            _chargeCreateHistoryAggregateService = chargeCreateHistoryAggregateService;
            _deleteChargesService = deleteChargesService;
            _tracer = tracer;
            _scopeFactory = scopeFactory;
            _chargeReadModel = chargeReadModel;
            _orderReadModel = orderReadModel;
            _projectService = projectService;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            foreach (var dto in dtos)
            {
                ProcessChargesInfo((ChargesInfoServiceBusDto)dto);
            }
        }

        private void ProcessChargesInfo(ChargesInfoServiceBusDto chargesInfo)
        {
            var timePeriod = new TimePeriod(chargesInfo.StartDate, chargesInfo.EndDate);

            var blockingWithdrawal = _accountReadModel.GetBlockingWithdrawals(chargesInfo.BranchCode, timePeriod);
            if (blockingWithdrawal.Any())
            {
                throw new CannotCreateChargesException(
                    string.Format("Can't create charges. The following organization units have in-progress or reverting withdrawals: {0}.",
                                  string.Join(", ",
                                              blockingWithdrawal.Select(x => string.Format("[{0} - {1} - {2}]",
                                                                                           x.OrganizationUnitId,
                                                                                           x.OrganizationUnitName,
                                                                                           x.WithdrawalStatus)))));
            }

            var chargesWithNegativeAmount = chargesInfo.Charges.Where(x => x.Amount < 0).ToArray();
            if (chargesWithNegativeAmount.Any())
            {
                throw new CannotCreateChargesException(string.Format("Can't import charges. Amount for following OrderPositions is negative: {0}",
                                                                     string.Join(",", chargesWithNegativeAmount.Select(x => x.OrderPositionId.ToString()))));
            }

            try
            {
                ProcessInScope(chargesInfo, timePeriod);
            }
            catch (Exception e)
            {
                _tracer.Error(e, e.Message);
                using (var transaction = new TransactionScope(TransactionScopeOption.Suppress, DefaultTransactionOptions.Default))
                {
                    LogImportStatus(chargesInfo, ChargesHistoryStatus.Error, e.Message);
                    transaction.Complete();
                }

                throw new NonBlockingImportErrorException("An error occurred during charges import.", e);
            }
        }

        private void ProcessInScope(ChargesInfoServiceBusDto chargesInfo, TimePeriod timePeriod)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ImportChargesInfoIdentity>())
            {
                Guid lastSucceededId;
                if (_chargeReadModel.TryGetLastChargeHistoryId(chargesInfo.BranchCode,
                                                                       timePeriod,
                                                                       ChargesHistoryStatus.Succeeded,
                                                                       out lastSucceededId))
                {
                    if (_accountReadModel.AnyLockDetailsCreated(lastSucceededId))
                    {
                        throw new CannotCreateChargesException(
                            string.Format("Can't create charges. Charges with sessionId = {0} have been used for lock details creation.", lastSucceededId));
                    }
                }

                if (!_projectService.DoesActiveProjectExist(chargesInfo.BranchCode))
                {
                    throw new CannotCreateChargesException(string.Format("Can't create charges. Project with code {0} not found",
                                                                         chargesInfo.BranchCode));
                }

                var specifiedOrderPositionIds = chargesInfo.Charges.Select(x => x.OrderPositionId).ToArray();

                var existingOrderPositions = _orderReadModel.GetExistingOrderPositionIds(specifiedOrderPositionIds);
                var missingOrderPositions = specifiedOrderPositionIds.Except(existingOrderPositions);
                if (missingOrderPositions.Any())
                {
                    throw new CannotCreateChargesException(
                        string.Format("Can't create charges. Following OrderPositions not found: {0}",
                                      string.Join(";", missingOrderPositions.Select(x => x.ToString()))));
                }

                var inactiveOrderPositions = _orderReadModel.PickInactiveOrDeletedOrderPositionNames(chargesInfo.Charges.Select(x => x.OrderPositionId).ToArray());
                if (inactiveOrderPositions.Any())
                {
                    throw new CannotCreateChargesException(
                        string.Format("Can't create charges. Following OrderPositions are inactive: {0}",
                                      string.Join(";", inactiveOrderPositions.Select(x => string.Format("{0} ({1})", x.Value, x.Key)))));
                }

                _deleteChargesService.Delete(chargesInfo.BranchCode, timePeriod, chargesInfo.SessionId);

                _chargeBulkCreateAggregateService.Create(chargesInfo.BranchCode,
                                                          chargesInfo.StartDate,
                                                          chargesInfo.EndDate,
                                                          chargesInfo.Charges,
                                                          chargesInfo.SessionId);

                LogImportStatus(chargesInfo, ChargesHistoryStatus.Succeeded, null);
                scope.Complete();
            }
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

            _chargeCreateHistoryAggregateService.Create(chargesHistoryDto);
        }
    }
}