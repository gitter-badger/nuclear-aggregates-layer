using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivatePricePositionService : IDeactivateGenericEntityService<PricePosition>
    {
        private readonly IPriceRepository _priceRepository;

        public DeactivatePricePositionService(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var deactivateAggregateRepository = _priceRepository as IDeactivateAggregateRepository<PricePosition>;
                deactivateAggregateRepository.Deactivate(entityId);

                transaction.Complete();
            }
            return null;
        }
    }
}