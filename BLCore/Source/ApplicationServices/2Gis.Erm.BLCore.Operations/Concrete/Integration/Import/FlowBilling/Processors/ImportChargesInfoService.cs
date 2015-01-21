using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.ReadModel;
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

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowBilling.Processors
{
    public class ImportChargesInfoService : IImportChargesInfoService
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IChargeBulkCreateAggregateService _chargeBulkCreateAggregateService;
        private readonly IChargeCreateHistoryAggregateService _chargeCreateHistoryAggregateService;
        private readonly IDeleteChargesForPeriodAndProjectOperationService _deleteChargesService;
        private readonly ICommonLog _logger;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IChargeReadModel _chargeReadModel;

        public ImportChargesInfoService(IAccountReadModel accountReadModel,
                                        IChargeBulkCreateAggregateService chargeBulkCreateAggregateService,
                                        IChargeCreateHistoryAggregateService chargeCreateHistoryAggregateService,
                                        IDeleteChargesForPeriodAndProjectOperationService deleteChargesService,
                                        ICommonLog logger,
                                        IOperationScopeFactory scopeFactory,
                                        IChargeReadModel chargeReadModel)
        {
            _accountReadModel = accountReadModel;
            _chargeBulkCreateAggregateService = chargeBulkCreateAggregateService;
            _chargeCreateHistoryAggregateService = chargeCreateHistoryAggregateService;
            _deleteChargesService = deleteChargesService;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _chargeReadModel = chargeReadModel;
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

            try
            {
                ProcessInScope(chargesInfo, timePeriod);
            }
            catch (Exception e)
            {
                _logger.ErrorEx(e, e.Message);
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

                // TODO {y.baranihin, 20.01.2015}: Добавить проверку на активность позиций заказа

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