using System;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Platforms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeletePlatformService : IDeleteGenericEntityService<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IPlatformService _platformService;

        public DeletePlatformService(IPlatformService platformService, IPositionRepository positionRepository)
        {
            _platformService = platformService;
            _positionRepository = positionRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            if (_platformService.IsPlatformLinked(entityId))
            {
                throw new NotificationException(BLResources.DeletePlatformService_Delete_CannotDeleteLinkidPlatform);
            }

            var platformInfo = _platformService.GetPlatformWithPositions(entityId);
            if (platformInfo == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var position in platformInfo.Positions)
                {
                    var positionClosure = position;
                    positionClosure.Platform = null;
                    _positionRepository.CreateOrUpdate(positionClosure);
                }

                _platformService.Delete(platformInfo.Platform);

                transaction.Complete();
                return null;
            }
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var platformInfo = _platformService.GetPlatform(entityId);

            if (platformInfo == null || _platformService.IsPlatformLinked(entityId))
            {
                return new DeleteConfirmationInfo { IsDeleteAllowed = false, DeleteDisallowedReason = BLResources.EntityNotFound };
            }

            return new DeleteConfirmationInfo
                   {
                       EntityCode = platformInfo.Name,
                       IsDeleteAllowed = true,
                       DeleteConfirmation = string.Empty
                   };
        }
    }
}