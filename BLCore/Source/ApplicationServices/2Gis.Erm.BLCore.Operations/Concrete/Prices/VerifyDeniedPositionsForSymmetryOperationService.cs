using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public sealed class VerifyDeniedPositionsForSymmetryOperationService : IVerifyDeniedPositionsForSymmetryOperationService
    {
        private readonly IPositionReadModel _positionReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public VerifyDeniedPositionsForSymmetryOperationService(IPositionReadModel positionReadModel, IOperationScopeFactory operationScopeFactory)
        {
            _positionReadModel = positionReadModel;
            _operationScopeFactory = operationScopeFactory;
        }

        public void VerifyWithinCollection(IEnumerable<DeniedPosition> deniedPositions)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<VerifyDeniedPositionsForSymmetryIdentity>())
            {
                var rulesWithoutSymmetricOne = deniedPositions.PickRulesWithoutSymmetricOnes();

                if (rulesWithoutSymmetricOne.Any())
                {
                    var positionNames =
                        _positionReadModel.GetPositionNames(rulesWithoutSymmetricOne.Select(x => x.PositionId)
                                                                                    .Concat(rulesWithoutSymmetricOne.Select(x => x.PositionDeniedId)));

                    throw new SymmetricDeniedPositionIsMissingException(string.Format(BLResources.SymmetricDeniedPositionIsMissing,
                                                                                      string.Join(",",
                                                                                                  rulesWithoutSymmetricOne.Select(x =>
                                                                                                                                  string.Format("({0}, {1})",
                                                                                                                                                positionNames[x.PositionId],
                                                                                                                                                positionNames[x.PositionDeniedId])))));
                }

                scope.Complete();
            }
        }
    }
}
