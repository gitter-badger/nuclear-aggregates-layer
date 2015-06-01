using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountWithdrawalChangeStatusAggregateService : IAccountWithdrawalChangeStatusAggregateService
    {
        private readonly IRepository<WithdrawalInfo> _withdrawalRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public AccountWithdrawalChangeStatusAggregateService(
            IRepository<WithdrawalInfo> withdrawalRepository, 
            IOperationScopeFactory scopeFactory)
        {
            _withdrawalRepository = withdrawalRepository;
            _scopeFactory = scopeFactory;
        }

        public void ChangeStatus(
            WithdrawalInfo withdrawal, 
            WithdrawalStatus targetStatus, 
            string changesDescription)
        {
            withdrawal.Status = targetStatus;
            withdrawal.Comment = changesDescription;

            ChangeStatus(withdrawal);
        }

        public void Finish(
            WithdrawalInfo withdrawal, 
            WithdrawalStatus targetStatus, 
            string changesDescription)
        {
            var sourceStatus = withdrawal.Status;
            if (sourceStatus != WithdrawalStatus.Withdrawing
                || (targetStatus != WithdrawalStatus.Error && targetStatus != WithdrawalStatus.Success))
            {
                throw new ArgumentException(string.Format("Check specified source and target withdrawal statuses. Source: {0}. Target: {1}", sourceStatus, targetStatus));
            }

            withdrawal.Status = targetStatus;
            withdrawal.Comment = changesDescription;
            withdrawal.FinishDate = DateTime.UtcNow;

            ChangeStatus(withdrawal);
        }

        private void ChangeStatus(WithdrawalInfo withdrawal)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, WithdrawalInfo>())
            {
                _withdrawalRepository.Update(withdrawal);
                scope.Updated<WithdrawalInfo>(withdrawal.Id);

                _withdrawalRepository.Save();
                scope.Complete();
            }
        }
    }
}