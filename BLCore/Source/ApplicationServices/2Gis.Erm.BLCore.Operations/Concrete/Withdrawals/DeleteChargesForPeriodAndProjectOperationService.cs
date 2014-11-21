using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.ReadModel;
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
        private readonly IChargeBulkDeleteAggregateService _chargeBulkDeleteAggregateService;
        private readonly IChargeCreateHistoryAggregateService _chargeCreateHistoryAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IChargeReadModel _chargeReadModel;

        public DeleteChargesForPeriodAndProjectOperationService(IChargeBulkDeleteAggregateService chargeBulkDeleteAggregateService,
                                                                IChargeCreateHistoryAggregateService chargeCreateHistoryAggregateService,
                                                                IOperationScopeFactory scopeFactory,
                                                                IChargeReadModel chargeReadModel)
        {
            _chargeBulkDeleteAggregateService = chargeBulkDeleteAggregateService;
            _chargeCreateHistoryAggregateService = chargeCreateHistoryAggregateService;
            _scopeFactory = scopeFactory;
            _chargeReadModel = chargeReadModel;
        }

        public void Delete(long projectId, TimePeriod timePeriod, Guid sessionId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<DeleteChargesForPeriodAndProjectIdentity>())
            {
                var chargesToDelete = _chargeReadModel.GetChargesToDelete(projectId, timePeriod);
                if (chargesToDelete.Any())
                {
                    var deletedSessionIds = chargesToDelete.Select(x => x.SessionId).Distinct().ToArray();
                    if (deletedSessionIds.Length != 1)
                    {
                        throw new MultipleChargesSessionsFoundException(string.Format("More than one sessionId found for period = {0} and projectId = {1}",
                                                                       timePeriod,
                                                                       projectId));
                    }

                    _chargeBulkDeleteAggregateService.Delete(chargesToDelete);

                    LogDeleted(projectId, timePeriod, deletedSessionIds[0], sessionId);
                }

                scope.Complete();
            }
        }

        private void LogDeleted(long projectId, TimePeriod timePeriod, Guid deletedSessionId, Guid currentSessionId)
        {
            var deletedMessage = _chargeReadModel.GetChargesHistoryMessage(deletedSessionId, ChargesHistoryStatus.Succeeded);
            var chargesHistoryDto = new ChargesHistoryDto
                {
                    ProjectId = projectId,
                    PeriodStartDate = timePeriod.Start,
                    PeriodEndDate = timePeriod.End,
                    Message = deletedMessage,
                    Status = ChargesHistoryStatus.Deleted,
                    SessionId = currentSessionId
                };

            _chargeCreateHistoryAggregateService.Create(chargesHistoryDto);
        }
    }
}