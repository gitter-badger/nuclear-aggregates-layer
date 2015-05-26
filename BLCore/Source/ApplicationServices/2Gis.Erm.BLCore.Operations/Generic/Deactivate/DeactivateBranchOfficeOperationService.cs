using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateBranchOfficeOperationService : IDeactivateGenericEntityService<BranchOffice>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;

        public DeactivateBranchOfficeOperationService(IBranchOfficeRepository branchOfficeRepository)
        {
            _branchOfficeRepository = branchOfficeRepository;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var deactivateAggregateRepository = _branchOfficeRepository as IDeactivateAggregateRepository<BranchOffice>;
                deactivateAggregateRepository.Deactivate(entityId);
                transaction.Complete();

                return null;
            }
        }
    }
}