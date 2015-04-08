using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class UpdateDeniedPositionAggregateService : IUpdateDeniedPositionAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<DeniedPosition> _deniedPositionRepository;

        public UpdateDeniedPositionAggregateService(IOperationScopeFactory operationScopeFactory, ISecureRepository<DeniedPosition> deniedPositionRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _deniedPositionRepository = deniedPositionRepository;
        }

        public void Update(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition)
        {
            if (deniedPosition == null)
            {
                throw new ArgumentNullException("deniedPosition");
            }

            if (symmetricDeniedPosition == null)
            {
                throw new ArgumentNullException("symmetricDeniedPosition");
            }

            if (deniedPosition.IsSelfDenied())
            {
                throw new NonSelfDeniedPositionExpectedException();
            }

            if (!deniedPosition.IsSymmetricTo(symmetricDeniedPosition))
            {
                throw new SymmetricDeniedPositionExpectedException();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, DeniedPosition>())
            {
                _deniedPositionRepository.Update(deniedPosition);
                _deniedPositionRepository.Update(symmetricDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Updated(deniedPosition)
                              .Updated(symmetricDeniedPosition)
                              .Complete();
            }
        }

        public void UpdateSelfDeniedPosition(DeniedPosition selfDeniedPosition)
        {
            if (selfDeniedPosition == null)
            {
                throw new ArgumentNullException("selfDeniedPosition");
            }

            if (!selfDeniedPosition.IsSelfDenied())
            {
                throw new SelfDeniedPositionExpectedException();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, DeniedPosition>())
            {
                _deniedPositionRepository.Update(selfDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Updated(selfDeniedPosition)
                              .Complete();
            }
        }
    }
}