using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public sealed class DeniedPositionsDuplicatesVerifier : IDeniedPositionsDuplicatesVerifier
    {
        private const int RulesToShowLimiter = 50;
        private readonly IPositionReadModel _positionReadModel;
        private readonly IPriceReadModel _priceReadModel;

        public DeniedPositionsDuplicatesVerifier(IPositionReadModel positionReadModel, IPriceReadModel priceReadModel)
        {
            _positionReadModel = positionReadModel;
            _priceReadModel = priceReadModel;
        }

        public void VerifyForDuplicates(DeniedPosition deniedPosition)
        {
            var duplicates =
                _priceReadModel.GetDeniedPositionsOrSymmetricDuplicates(deniedPosition.Id,
                                                                        deniedPosition.PositionId,
                                                                        deniedPosition.PositionDeniedId,
                                                                        deniedPosition.PriceId)
                               .ToArray();

            if (duplicates.Any())
            {
                ThrowError(duplicates);
            }
        }

        public void VerifyForDuplicatesWithinCollection(IEnumerable<DeniedPosition> deniedPositions)
        {
            var duplicateRules = deniedPositions.GroupBy(x => new
                                                                  {
                                                                      x.PositionId,
                                                                      x.PositionDeniedId,
                                                                  })
                                                .Where(x => x.Count() > 1)
                                                .SelectMany(x => x)
                                                .ToArray();
            if (duplicateRules.Any())
            {
                ThrowError(duplicateRules);
            }
        }

        private void ThrowError(IEnumerable<DeniedPosition> duplicates)
        {
            var keys = duplicates.GroupBy(x => new
                                                   {
                                                       x.PositionId,
                                                       x.PositionDeniedId,
                                                   })
                                 .Select(x => x.Key)
                                 .Take(RulesToShowLimiter)
                                 .ToArray();

            var positionNames = _positionReadModel.GetPositionNames(keys.Select(x => x.PositionId)
                                                                        .Concat(keys.Select(x => x.PositionDeniedId)));

            throw new EntityIsNotUniqueException(typeof(DeniedPosition),
                                                 string.Format(BLResources.DuplicateDeniedPositionsAreFound,
                                                               string.Join(",",
                                                                           keys.Select(x =>
                                                                                       string.Format("({0}, {1})",
                                                                                                     positionNames[x.PositionId],
                                                                                                     positionNames[x.PositionDeniedId])))));
        }
    }
}
