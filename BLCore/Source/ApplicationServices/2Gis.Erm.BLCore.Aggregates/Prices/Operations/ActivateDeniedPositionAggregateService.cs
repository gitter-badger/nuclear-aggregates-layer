using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class ActivateDeniedPositionAggregateService : IActivateDeniedPositionAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<DeniedPosition> _deniedPositionRepository;

        public ActivateDeniedPositionAggregateService(IOperationScopeFactory operationScopeFactory, ISecureRepository<DeniedPosition> deniedPositionRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _deniedPositionRepository = deniedPositionRepository;
        }

        public void Activate(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition)
        {
            if (deniedPosition.IsSelfDenied())
            {
                throw new NonSelfDeniedPositionExpectedException();
            }

            if (symmetricDeniedPosition == null)
            {
                throw new SymmetricDeniedPositionExpectedException();
            }

            if (!deniedPosition.IsSymmetricTo(symmetricDeniedPosition))
            {
                throw new SymmetricDeniedPositionExpectedException();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, DeniedPosition>())
            {
                deniedPosition.IsActive = true;
                symmetricDeniedPosition.IsActive = true;
                _deniedPositionRepository.Update(deniedPosition);
                _deniedPositionRepository.Update(symmetricDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Updated(deniedPosition)
                              .Updated(symmetricDeniedPosition)
                              .Complete();
            }
        }

        public void ActivateSelfDeniedPosition(DeniedPosition selfDeniedPosition)
        {
            if (!selfDeniedPosition.IsSelfDenied())
            {
                throw new SelfDeniedPositionExpectedException();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, DeniedPosition>())
            {
                selfDeniedPosition.IsActive = true;
                _deniedPositionRepository.Update(selfDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Updated(selfDeniedPosition)
                              .Complete();
            }
        }
    }
}