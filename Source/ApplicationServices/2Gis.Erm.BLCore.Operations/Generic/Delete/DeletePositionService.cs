using System;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeletePositionService : IDeleteGenericEntityService<Position>
    {
        private readonly IPositionRepository _positionRepository;

        public DeletePositionService(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var position = _positionRepository.GetPosition(entityId);

            if (position == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }

            // todo: masterPositionNames.First - эмуляция существовавшей раньше логики. Возможно, не лучший выбор и стоит подумать, что будет лучше.
            var masterPositionNames = _positionRepository.GetMasterPositionNames(position);
            if (masterPositionNames.Any())
            {
                throw new ArgumentException(string.Format(BLResources.PositionIsUsedInCompositePosition, masterPositionNames.First())); 
            }
            if (_positionRepository.IsInPublishedPrices(entityId))
            {
                throw new ArgumentException(BLResources.ErrorCantDeletePositionInPublishedPrice);
            }

            // todo: иногда мне кажется, что логично было бы обработать транзакцию внутри метода TransactionScope, но это почему-то не пока принято.
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _positionRepository.DeleteWithSubentities(position);
                transaction.Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var position = _positionRepository.GetPosition(entityId);
            if (position == null)
            {
                return new DeleteConfirmationInfo
                    {
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = string.Format(BLResources.EntityNotFound)
                    };
            }

            var masterPositionNames = _positionRepository.GetMasterPositionNames(position);
            if (masterPositionNames.Any())
            {
                return new DeleteConfirmationInfo
                       {
                           IsDeleteAllowed = false,
                           DeleteDisallowedReason = string.Format(BLResources.PositionIsUsedInCompositePosition, masterPositionNames.First())
                       };
            }

            return new DeleteConfirmationInfo
                {
                    EntityCode = position.Name,
                    IsDeleteAllowed = true,
                    DeleteConfirmation = string.Empty
                };
        }
    }
}