using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeletePositionCategoryService : IDeleteGenericEntityService<PositionCategory>
    {
        private readonly IPositionRepository _positionRepository;

        public DeletePositionCategoryService(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var categoryInfo = _positionRepository.GetCategoryWithPositions(entityId);

            if (categoryInfo.PositionCategory == null)
            {
                throw new NotificationException(BLResources.EntityNotFound);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var position in categoryInfo.Positions)
                {
                    var positionClosure = position;
                    positionClosure.Platform = null;
                    _positionRepository.CreateOrUpdate(positionClosure);
                }

                _positionRepository.Delete(categoryInfo.PositionCategory);

                transaction.Complete();
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