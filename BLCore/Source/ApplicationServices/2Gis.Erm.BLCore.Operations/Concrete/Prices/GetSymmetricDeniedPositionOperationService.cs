using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
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

        public DeniedPosition GetTheOnlyOneOrDie(long positionId, long positionDeniedId, long priceId)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetSymmetricDeniedPositionIdentity>())
            {
                var symmetricDeniedPositions =
                    _priceReadModel.GetDeniedPositions(positionId: positionDeniedId,
                                                       positionDeniedId: positionId,
                                                       priceId: priceId);
                                   
                EnsureUniqueness(symmetricDeniedPositions);
                EnsureExistence(positionId, positionDeniedId, symmetricDeniedPositions);
                scope.Complete();

                return symmetricDeniedPositions.Single();
            }
        }

        public DeniedPosition GetTheOnlyOneWithObjectBindingTypeConsiderationOrDie(long positionId, long positionDeniedId, long priceId, ObjectBindingType objectBindingType)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetSymmetricDeniedPositionIdentity>())
            {
                var symmetricDeniedPositions =
                    _priceReadModel.GetDeniedPositions(positionId: positionDeniedId,
                                                       positionDeniedId: positionId,
                                                       priceId: priceId,
                                                       objectBindingType: objectBindingType);

                EnsureUniqueness(symmetricDeniedPositions);
                EnsureExistence(positionId, positionDeniedId, symmetricDeniedPositions);
                scope.Complete();

                return symmetricDeniedPositions.Single();
            }
        }

        private void EnsureExistence(long positionId, long positionDeniedId, IEnumerable<DeniedPosition> symmetricDeniedPositions)
        {
            if (!symmetricDeniedPositions.Any())
            {
                var positionNames = _positionReadModel.GetPositionNames(new[] { positionId, positionDeniedId });
                throw new SymmetricDeniedPositionIsMissingException(string.Format(BLResources.SymmetricDeniedPositionIsMissing,
                                                                                  string.Format("({0}, {1})",
                                                                                                positionNames[positionId],
                                                                                                positionNames[positionDeniedId])));
            }
        }

        private void EnsureUniqueness(IReadOnlyCollection<DeniedPosition> symmetricDeniedPositions)
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
