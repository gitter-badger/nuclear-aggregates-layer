using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Platforms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeletePlatformService : IDeleteGenericEntityService<Platform.Model.Entities.Erm.Platform>
    {
        private readonly IPlatformService _platformService;
        private readonly IPositionRepository _positionRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeletePlatformService(IPlatformService platformService, IPositionRepository positionRepository, IOperationScopeFactory scopeFactory)
        {
            _platformService = platformService;
            _positionRepository = positionRepository;
            _scopeFactory = scopeFactory;
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

            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Platform.Model.Entities.Erm.Platform>())
            {
                foreach (var position in platformInfo.Positions)
                {
                    position.Platform = null;
                    _positionRepository.CreateOrUpdate(position);
                    scope.Updated(position);
                }

                _platformService.Delete(platformInfo.Platform);
                scope.Deleted(platformInfo.Platform);

                scope.Complete();
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