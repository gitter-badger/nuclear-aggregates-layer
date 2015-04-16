using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateAssociatedPositionsGroupOperationService : IActivateGenericEntityService<AssociatedPositionsGroup>
    {
        private readonly IPriceRepository _priceRepository;

        public ActivateAssociatedPositionsGroupOperationService(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public int Activate(long entityId)
        {
            int result = 0;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var activateAggregateRepository = _priceRepository as IActivateAggregateRepository<AssociatedPositionsGroup>;
                result = activateAggregateRepository.Activate(entityId);

                transaction.Complete();
            }
            return result;
        }

        #region Implementation of IDeactivateEntityService

        public DeactivateConfirmation Deactivate(int entityId, int ownerCode)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
