using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteLimitService : IDeleteGenericEntityService<Limit>
    {
        private readonly IAccountRepository _accountRepository;

        public DeleteLimitService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var limit = _accountRepository.FindLimit(entityId);
            if (limit == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }
            if (!limit.IsActive)
            {
                throw new ArgumentException(BLResources.CantDeleteInactiveLimit);
            }

            _accountRepository.Delete(limit);
            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var limit = _accountRepository.FindLimit(entityId);
            if (limit == null)
            {
                return new DeleteConfirmationInfo
                    {
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.EntityNotFound
                    };
            }
            if (!limit.IsActive)
            {
                return new DeleteConfirmationInfo
                    {
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.CantDeleteInactiveLimit
                    };
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