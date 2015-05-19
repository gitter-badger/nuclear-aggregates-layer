using System;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteLockDetailOperationService : IDeleteGenericEntityService<LockDetail>
    {
        private readonly IAccountRepository _accountAggregateRepository;

        public DeleteLockDetailOperationService(IAccountRepository accountAggregateRepository)
        {
            _accountAggregateRepository = accountAggregateRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var lockDetailInfo = _accountAggregateRepository.GetLockDetail(entityId);
            if (lockDetailInfo == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _accountAggregateRepository.Delete(lockDetailInfo.LockDetail);
                _accountAggregateRepository.RecalculateLockValue(lockDetailInfo.Lock);
                _accountAggregateRepository.Update(lockDetailInfo.Lock);

                transaction.Complete();
                return null;
            }
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var lockDetailInfo = _accountAggregateRepository.GetLockDetail(entityId);

            if (lockDetailInfo == null)
            {
                return new DeleteConfirmationInfo { IsDeleteAllowed = false, DeleteDisallowedReason = BLResources.EntityNotFound };
            }

            return new DeleteConfirmationInfo
            {
                EntityCode = string.Empty,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}