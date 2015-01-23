using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel;
using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Operations.Generic.Deactivate
{
    public class DeactivateDenialReasonOperationService : IDeactivateGenericEntityService<DenialReason>
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IDeactivateDenialReasonService _deactivateService;
        private readonly IDenialReasonReadModel _denialReasonReadModel;

        public DeactivateDenialReasonOperationService(IOperationScopeFactory scopeFactory,
                                                                   IDeactivateDenialReasonService deactivateService,
                                                                   IDenialReasonReadModel denialReasonReadModel)
        {
            _scopeFactory = scopeFactory;
            _deactivateService = deactivateService;
            _denialReasonReadModel = denialReasonReadModel;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeactivateIdentity, DenialReason>())
            {
                _deactivateService.Deactivate(_denialReasonReadModel.GetDenialReason(entityId));

                scope.Updated<DenialReason>(entityId)
                     .Complete();
            }

            return null;
        }
    }
}