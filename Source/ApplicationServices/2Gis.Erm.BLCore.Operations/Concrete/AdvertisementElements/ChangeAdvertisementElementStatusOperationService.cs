using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements
{
    public class ChangeAdvertisementElementStatusOperationService : IChangeAdvertisementElementStatusOperationService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly IChangeAdvertisementElementStatusStrategiesFactory _strategiesFactory;
        private readonly IActionLogger _actionLogger;

        public ChangeAdvertisementElementStatusOperationService(
            IOperationScopeFactory operationScopeFactory,
            IAdvertisementReadModel advertisementReadModel,
            IChangeAdvertisementElementStatusStrategiesFactory strategiesFactory,
            IActionLogger actionLogger)
        {
            _operationScopeFactory = operationScopeFactory;
            _advertisementReadModel = advertisementReadModel;
            _strategiesFactory = strategiesFactory;
            _actionLogger = actionLogger;
        }

        public void ChangeStatus(long advertisementElementId,
                                 AdvertisementElementStatusValue newStatus,
                                 IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            var advertisementElementStatusInfo = _advertisementReadModel.GetAdvertisementElementValidationState(advertisementElementId);
            if (!advertisementElementStatusInfo.NeedsValidation)
            {
                throw new NotVerifiableAdvertisementElementStatusChangingException(BLResources.YouMustNotChangeStatusOfNotVerifiableAdvertisementElement);
            }

            var currentStatus = advertisementElementStatusInfo.CurrentStatus;
            var currentStatusValue = (AdvertisementElementStatusValue)currentStatus.Status;
            var strategies = _strategiesFactory.EvaluateProcessingStrategies(currentStatusValue,
                                                                             newStatus);
            using (var scope = _operationScopeFactory.CreateNonCoupled<ChangeAdvertisementElementStatusIdentity>())
            {
                foreach (var strategy in strategies)
                {
                    strategy.Validate(currentStatus, denialReasons);
                    strategy.Process(currentStatus, denialReasons);
                }

                scope.Complete();
            }

            _actionLogger.LogChanges(currentStatus,
                                     x => x.Status,
                                     (int)currentStatusValue,
                                     currentStatus.Status);
        }
    }
}