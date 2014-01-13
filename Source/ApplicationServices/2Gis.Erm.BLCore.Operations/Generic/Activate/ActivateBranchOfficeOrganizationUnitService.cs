using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateBranchOfficeOrganizationUnitService : IActivateGenericEntityService<BranchOfficeOrganizationUnit>
    {
        private readonly IBranchOfficeRepository _branchOfficeRepository;

        public ActivateBranchOfficeOrganizationUnitService(IBranchOfficeRepository branchOfficeRepository)
        {
            _branchOfficeRepository = branchOfficeRepository;
        }

        public int Activate(long entityId)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var activateAggregateRepository = (IActivateAggregateRepository<BranchOfficeOrganizationUnit>)_branchOfficeRepository;
                var result = activateAggregateRepository.Activate(entityId);

                transaction.Complete();

                return result;
            }
        }
    }
}