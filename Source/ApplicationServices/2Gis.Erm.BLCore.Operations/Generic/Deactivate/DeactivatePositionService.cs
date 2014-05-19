using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivatePositionService : IDeactivateGenericEntityService<Position>
    {
        private readonly IPositionRepository _positionRepository;

        public DeactivatePositionService(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var deactivateAggregateRepository = _positionRepository as IDeactivateAggregateRepository<Position>;
                deactivateAggregateRepository.Deactivate(entityId);

                transaction.Complete();
            }

            return null;
        }
    }
}