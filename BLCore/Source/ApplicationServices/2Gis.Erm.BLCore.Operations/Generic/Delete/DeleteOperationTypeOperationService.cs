using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteOperationTypeOperationService : IDeleteGenericEntityService<OperationType>
    {
        private readonly IAccountRepository _accountRepository;

        public DeleteOperationTypeOperationService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var operationTypeInfo = _accountRepository.GetOperationTypeDto(entityId);
            if (operationTypeInfo == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }
            if (!operationTypeInfo.AllAccountDetailsIsDeleted)
            {
                throw new ArgumentException(BLResources.CantDeleteOperationType);
            }

            _accountRepository.Delete(operationTypeInfo.OperationType);
            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var operationTypeInfo = _accountRepository.GetOperationTypeDto(entityId);
            if (operationTypeInfo == null)
            {
                return new DeleteConfirmationInfo { IsDeleteAllowed = false, DeleteDisallowedReason = BLResources.EntityNotFound };
            }
            if (!operationTypeInfo.AllAccountDetailsIsDeleted)
            {
                return new DeleteConfirmationInfo { IsDeleteAllowed = false, DeleteDisallowedReason = BLResources.CantDeleteOperationType };
            }

            return new DeleteConfirmationInfo
                   {
                       EntityCode = operationTypeInfo.OperationType.Name,
                       IsDeleteAllowed = true,
                       DeleteConfirmation = string.Empty
                   };
        }
    }
}