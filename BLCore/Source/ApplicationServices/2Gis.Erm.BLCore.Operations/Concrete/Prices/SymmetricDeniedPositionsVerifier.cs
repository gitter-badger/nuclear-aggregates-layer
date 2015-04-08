using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public sealed class SymmetricDeniedPositionsVerifier : ISymmetricDeniedPositionsVerifier
    {
        private readonly IPositionReadModel _positionReadModel;

        public SymmetricDeniedPositionsVerifier(IPositionReadModel positionReadModel)
        {
            _positionReadModel = positionReadModel;
        }

        public void VerifyForSymmetryWithinCollection(IEnumerable<DeniedPosition> deniedPositions)
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
        }
    }
}
