using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Delete
{
    public sealed class DeleteBankService : IDeleteGenericEntityService<Bank>, IChileAdapted
    {
        private readonly IBankService _bankService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IBankReadModel _readModel;

        public DeleteBankService(
            IOperationScopeFactory scopeFactory, 
            IBankReadModel readModel,
            IBankService bankService)
        {
            _scopeFactory = scopeFactory;
            _readModel = readModel;
            _bankService = bankService;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Bank>())
            {
                var bankUsed = _readModel.IsBankUsed(entityId);
                if (bankUsed)
                {
                    throw new NotificationException(BLResources.CannotDeleteUsedBank);
                }

                var entity = _readModel.GetBank(entityId);
                _bankService.Delete(entity);
                scope.Deleted<Bank>(entityId)
                     .Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var bankUsed = _readModel.IsBankUsed(entityId);
            if (bankUsed)
            {
                return new DeleteConfirmationInfo
                    {
                        DeleteDisallowedReason = BLResources.CannotDeleteUsedBank,
                        IsDeleteAllowed = false
                    };
            }

            return null;
        }
    }
}
