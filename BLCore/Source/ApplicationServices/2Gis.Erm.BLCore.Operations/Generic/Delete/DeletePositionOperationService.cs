using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeletePositionOperationService : IDeleteGenericEntityService<Position>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeletePositionOperationService(IPositionRepository positionRepository, IOperationScopeFactory scopeFactory)
        {
            _positionRepository = positionRepository;
            _scopeFactory = scopeFactory;
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
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Position>())
            {
                _positionRepository.DeleteWithSubentities(position);

                scope.Deleted<Position>(entityId)
                     .Complete();
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