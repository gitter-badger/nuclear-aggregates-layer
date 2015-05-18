using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeletePositionCategoryService : IDeleteGenericEntityService<PositionCategory>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeletePositionCategoryService(IPositionRepository positionRepository, IOperationScopeFactory scopeFactory)
        {
            _positionRepository = positionRepository;
            _scopeFactory = scopeFactory;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var categoryInfo = _positionRepository.GetCategoryWithPositions(entityId);

            if (categoryInfo.PositionCategory == null)
            {
                throw new NotificationException(BLResources.EntityNotFound);
            }

            using (var scope = _scopeFactory.CreateSpecificFor<DetachIdentity, Position, Category>())
            {
                foreach (var position in categoryInfo.Positions)
                {
                    position.Platform = null;
                    _positionRepository.CreateOrUpdate(position);
                    scope.Updated(position);
                }

                _positionRepository.Delete(categoryInfo.PositionCategory);

                scope.Deleted(categoryInfo.PositionCategory)
                     .Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var categoryInfo = _positionRepository.GetCategoryWithPositions(entityId);

            if (categoryInfo == null)
            {
                return new DeleteConfirmationInfo { IsDeleteAllowed = false, DeleteDisallowedReason = BLResources.EntityNotFound };
            }

            return new DeleteConfirmationInfo
                {
                    EntityCode = categoryInfo.PositionCategory.Name,
                    IsDeleteAllowed = true,
                    DeleteConfirmation = string.Empty
                };
        }
    }
}