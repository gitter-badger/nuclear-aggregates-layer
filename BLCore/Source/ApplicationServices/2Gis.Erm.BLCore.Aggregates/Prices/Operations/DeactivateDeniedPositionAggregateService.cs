﻿using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Common.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class DeactivateDeniedPositionAggregateService : IDeactivateDeniedPositionAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<DeniedPosition> _deniedPositionRepository;

        public DeactivateDeniedPositionAggregateService(IOperationScopeFactory operationScopeFactory, ISecureRepository<DeniedPosition> deniedPositionRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _deniedPositionRepository = deniedPositionRepository;
        }

        public void Deactivate(DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition)
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

            if (!deniedPosition.IsActive)
            {
                throw new InactiveEntityDeactivationException(typeof(DeniedPosition), deniedPosition.Id);
            }

            if (!symmetricDeniedPosition.IsActive)
            {
                throw new InactiveEntityDeactivationException(typeof(DeniedPosition), symmetricDeniedPosition.Id);
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, DeniedPosition>())
            {
                deniedPosition.IsActive = false;
                symmetricDeniedPosition.IsActive = false;
                _deniedPositionRepository.Update(deniedPosition);
                _deniedPositionRepository.Update(symmetricDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Updated(deniedPosition)
                              .Updated(symmetricDeniedPosition)
                              .Complete();
            }
        }

        public void DeactivateSelfDenied(DeniedPosition selfDeniedPosition)
        {
            if (selfDeniedPosition == null)
            {
                throw new ArgumentNullException("selfDeniedPosition");
            }

            if (!selfDeniedPosition.IsSelfDenied())
            {
                throw new SelfDeniedPositionExpectedException();
            }

            if (!selfDeniedPosition.IsActive)
            {
                throw new InactiveEntityDeactivationException(typeof(DeniedPosition), selfDeniedPosition.Id);
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, DeniedPosition>())
            {
                selfDeniedPosition.IsActive = false;
                _deniedPositionRepository.Update(selfDeniedPosition);
                _deniedPositionRepository.Save();

                operationScope.Updated(selfDeniedPosition)
                              .Complete();
            }
        }
    }
}