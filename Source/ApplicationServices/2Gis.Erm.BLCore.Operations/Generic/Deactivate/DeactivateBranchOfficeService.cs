using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateBranchOfficeService : IDeactivateGenericEntityService<BranchOffice>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;

        public DeactivateBranchOfficeService(IBranchOfficeRepository branchOfficeRepository)
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