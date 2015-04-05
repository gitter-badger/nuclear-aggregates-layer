using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class CreateDeniedPositionAggregateService : ICreateDeniedPositionAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly ISecureRepository<DeniedPosition> _deniedPositionRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CreateDeniedPositionAggregateService(IIdentityProvider identityProvider,
                                                    ISecureRepository<DeniedPosition> deniedPositionRepository,
                                                    IOperationScopeFactory operationScopeFactory)
        {
            _identityProvider = identityProvider;
            _deniedPositionRepository = deniedPositionRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Create(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition)
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

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DeniedPosition>())
            {
                _identityProvider.SetFor(deniedPosition, symmetricDeniedPosition);
                _deniedPositionRepository.Add(deniedPosition);
                _deniedPositionRepository.Add(symmetricDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Added(deniedPosition)
                              .Added(symmetricDeniedPosition)
                              .Complete();
            }
        }

        public void CreateSelfDeniedPosition(DeniedPosition selfDeniedPosition)
        {
            if (!selfDeniedPosition.IsSelfDenied())
            {
                throw new SelfDeniedPositionExpectedException();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DeniedPosition>())
            {
                _identityProvider.SetFor(selfDeniedPosition);
                _deniedPositionRepository.Add(selfDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Added(selfDeniedPosition)
                              .Complete();
            }
        }

        /// <summary>
        /// Нужен для кейса редактирования правила запрещения со сменой направления c самозапрещения
        /// </summary>
        public void Create(DeniedPosition deniedPosition)
        {
            if (deniedPosition.IsSelfDenied())
            {
                throw new NonSelfDeniedPositionExpectedException();
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DeniedPosition>())
            {
                _identityProvider.SetFor(deniedPosition);
                _deniedPositionRepository.Add(deniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Added(deniedPosition)
                              .Complete();
            }
        }
    }
}