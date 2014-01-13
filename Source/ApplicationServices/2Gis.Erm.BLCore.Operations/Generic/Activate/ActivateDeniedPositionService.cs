using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateDeniedPositionService : IActivateGenericEntityService<DeniedPosition>
    {
        private readonly IPriceRepository _priceRepository;

        public ActivateDeniedPositionService(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public int Activate(long entityId)
        {
            int result = 0;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var activateAggregateRepository = _priceRepository as IActivateAggregateRepository<DeniedPosition>;
                activateAggregateRepository.Activate(entityId);

                transaction.Complete();
            }
            return result;
        }
    }
}
