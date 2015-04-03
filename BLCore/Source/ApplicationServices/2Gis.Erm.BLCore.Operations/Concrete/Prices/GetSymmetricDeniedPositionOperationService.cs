﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public class GetSymmetricDeniedPositionOperationService : IGetSymmetricDeniedPositionOperationService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IPositionReadModel _positionReadModel;

        public GetSymmetricDeniedPositionOperationService(IOperationScopeFactory operationScopeFactory, IPriceReadModel priceReadModel, IPositionReadModel positionReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceReadModel = priceReadModel;
            _positionReadModel = positionReadModel;
        }

        public DeniedPosition Get(long deniedPositionId)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetSymmetricDeniedPositionIdentity>())
            {
                var deniedPosition = _priceReadModel.GetDeniedPosition(deniedPositionId);
                if (deniedPosition == null)
                {
                    throw new EntityNotFoundException(typeof(DeniedPosition), deniedPositionId);
                }

                var symmetricDeniedPositions =
                    _priceReadModel.GetDeniedPositions(positionId: deniedPosition.PositionDeniedId, positionDeniedId: deniedPosition.PositionId, priceId: deniedPosition.PriceId)
                                   .ToArray();

                EnsureUniqueness(symmetricDeniedPositions);
                EnsureExistance(deniedPosition, symmetricDeniedPositions);
                scope.Complete();

                return symmetricDeniedPositions.Single();
            }
        }

        public DeniedPosition GetWithObjectBindingTypeConsideration(long deniedPositionId)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetSymmetricDeniedPositionIdentity>())
            {
                var deniedPosition = _priceReadModel.GetDeniedPosition(deniedPositionId);
                if (deniedPosition == null)
                {
                    throw new EntityNotFoundException(typeof(DeniedPosition), deniedPositionId);
                }

                var symmetricDeniedPositions =
                    _priceReadModel.GetDeniedPositions(positionId: deniedPosition.PositionDeniedId,
                                                       positionDeniedId: deniedPosition.PositionId,
                                                       priceId: deniedPosition.PriceId,
                                                       objectBindingType: deniedPosition.ObjectBindingType)
                                   .ToArray();

                EnsureUniqueness(symmetricDeniedPositions);
                EnsureExistance(deniedPosition, symmetricDeniedPositions);
                scope.Complete();

                return symmetricDeniedPositions.Single();
            }
        }

        public DeniedPosition GetInactiveWithObjectBindingTypeConsideration(long deniedPositionId)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetSymmetricDeniedPositionIdentity>())
            {
                var deniedPosition = _priceReadModel.GetDeniedPosition(deniedPositionId);
                if (deniedPosition == null)
                {
                    throw new EntityNotFoundException(typeof(DeniedPosition), deniedPositionId);
                }

                var symmetricDeniedPositions =
                    _priceReadModel.GetInactiveDeniedPositions(positionId: deniedPosition.PositionDeniedId,
                                                               positionDeniedId: deniedPosition.PositionId,
                                                               priceId: deniedPosition.PriceId,
                                                               objectBindingType: deniedPosition.ObjectBindingType)
                                   .ToArray();

                EnsureExistance(deniedPosition, symmetricDeniedPositions);
                scope.Complete();

                return symmetricDeniedPositions.First();
            }
        }

        private void EnsureExistance(DeniedPosition deniedPosition, IEnumerable<DeniedPosition> symmetricDeniedPositions)
        {
            if (!symmetricDeniedPositions.Any())
            {
                var positionNames = _positionReadModel.GetPositionNames(new[] { deniedPosition.PositionId, deniedPosition.PositionDeniedId });
                throw new SymmetricDeniedPositionIsMissingException(string.Format(BLResources.SymmetricDeniedPositionIsMissing,
                                                                                  string.Format("({0}, {1})",
                                                                                                positionNames[deniedPosition.PositionId],
                                                                                                positionNames[deniedPosition.PositionDeniedId])));
            }
        }

        private void EnsureUniqueness(IEnumerable<DeniedPosition> symmetricDeniedPositions)
        {
            if (symmetricDeniedPositions.Count() > 1)
            {
                var positionNames = _positionReadModel.GetPositionNames(symmetricDeniedPositions.Select(x => x.PositionId)
                                                                                                .Concat(symmetricDeniedPositions.Select(x => x.PositionDeniedId)));
                throw new EntityIsNotUniqueException(typeof(DeniedPosition),
                                                     string.Format(BLResources.DuplicateDeniedPositionsAreFound,
                                                                   string.Join(",",
                                                                               symmetricDeniedPositions.Select(x =>
                                                                                                               string.Format("({0}, {1})",
                                                                                                                             positionNames[x.PositionId],
                                                                                                                             positionNames[x.PositionDeniedId])))));
            }
        }
    }
}
