using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Withdrawals.Dto;
using DoubleGis.Erm.BLCore.Aggregates.Withdrawals.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Withdrawals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions.Withdrawal.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public class DeleteChargesForPeriodAndProjectOperationService : IDeleteChargesForPeriodAndProjectOperationService
    {
        private readonly IBulkDeleteChargesAggregateService _bulkDeleteChargesAggregateService;
        private readonly ICreateChargesHistoryAggregateService _createChargesHistoryAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IWithdrawalInfoReadModel _withdrawalInfoReadModel;

        public DeleteChargesForPeriodAndProjectOperationService(IBulkDeleteChargesAggregateService bulkDeleteChargesAggregateService,
                                                                ICreateChargesHistoryAggregateService createChargesHistoryAggregateService,
                                                                IOperationScopeFactory scopeFactory,
                                                                IWithdrawalInfoReadModel withdrawalInfoReadModel)
        {
            _bulkDeleteChargesAggregateService = bulkDeleteChargesAggregateService;
            _createChargesHistoryAggregateService = createChargesHistoryAggregateService;
            _scopeFactory = scopeFactory;
            _withdrawalInfoReadModel = withdrawalInfoReadModel;
        }

        public void Delete(long projectId, TimePeriod timePeriod, Guid sessionId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<DeleteChargesForPeriodAndProjectIdentity>())
            {
                var chargesToDelete = _withdrawalInfoReadModel.GetChargesToDelete(projectId, timePeriod);
                if (chargesToDelete.Any())
                {
                    var deletedSessionIds = chargesToDelete.Select(x => x.SessionId).Distinct().ToArray();
                    if (deletedSessionIds.Length != 1)
                    {
                        throw new MultipleChargesSessionsFoundException(string.Format("More than one sessionId found for period = {0} and projectId = {1}",
                                                                       timePeriod,
                                                                       projectId));
                    }

                    _bulkDeleteChargesAggregateService.Delete(chargesToDelete);

                    LogDeleted(projectId, timePeriod, deletedSessionIds[0], sessionId);
                }

                scope.Complete();
            }
        }

        private void LogDeleted(long projectId, TimePeriod timePeriod, Guid deletedSessionId, Guid currentSessionId)
        {
            var deletedMessage = _withdrawalInfoReadModel.GetChargesHistoryMessage(deletedSessionId, ChargesHistoryStatus.Succeeded);
            var chargesHistoryDto = new ChargesHistoryDto
                {
                    ProjectId = projectId,
                    PeriodStartDate = timePeriod.Start,
                    PeriodEndDate = timePeriod.End,
                    Message = deletedMessage,
                    Status = ChargesHistoryStatus.Deleted,
                    SessionId = currentSessionId
                };

            _createChargesHistoryAggregateService.Create(chargesHistoryDto);
        }
    }
}