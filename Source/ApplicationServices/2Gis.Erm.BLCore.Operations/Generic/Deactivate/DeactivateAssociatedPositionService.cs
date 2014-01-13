﻿using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateAssociatedPositionService : IDeactivateGenericEntityService<AssociatedPosition>
    {
        private readonly IPriceRepository _priceRepository;

        public DeactivateAssociatedPositionService(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var deactivateAggregateRepository = _priceRepository as IDeactivateAggregateRepository<AssociatedPosition>;
                deactivateAggregateRepository.Deactivate(entityId);

                transaction.Complete();
            }

            return null;
        }
    }
}