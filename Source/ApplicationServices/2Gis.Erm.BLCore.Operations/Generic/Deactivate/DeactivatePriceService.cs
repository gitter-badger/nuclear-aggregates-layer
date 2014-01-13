using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivatePriceService : IDeactivateGenericEntityService<Price>
    {
        private readonly IPriceRepository _priceRepository;

        public DeactivatePriceService(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var pricePublishedForToday = _priceRepository.PricePublishedForToday(entityId);
                if (pricePublishedForToday)
                {
                    throw new NotificationException(BLResources.PriceInActionCannotBeDeactivated);
                }

                var deactivateAggregateRepository = _priceRepository as IDeactivateAggregateRepository<Price>;
                deactivateAggregateRepository.Deactivate(entityId);

                transaction.Complete();
            }

            return null;
        }
    }
}