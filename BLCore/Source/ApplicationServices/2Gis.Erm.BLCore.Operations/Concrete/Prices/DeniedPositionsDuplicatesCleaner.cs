using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public sealed class DeniedPositionsDuplicatesCleaner : IDeniedPositionsDuplicatesCleaner
    {
        private const int RulesToShowLimiter = 50;
        private readonly IPositionReadModel _positionReadModel;

        public DeniedPositionsDuplicatesCleaner(IPositionReadModel positionReadModel)
        {
            _positionReadModel = positionReadModel;
        }

        public IEnumerable<DeniedPosition> Distinct(IEnumerable<DeniedPosition> deniedPositions)
        {
            return deniedPositions.Distinct(new DeniedPositionComparer()).ToArray();
        }

        public void VerifyForDuplicates(IEnumerable<DeniedPosition> deniedPositions)
        {
            var duplicateRules = deniedPositions.GroupBy(x => new
                                                                  {
                                                                      x.PositionId,
                                                                      x.PositionDeniedId,
                                                                  })
                                                .Where(x => x.Count() > 1)
                                                .Select(x => x.Key)
                                                .Take(RulesToShowLimiter)
                                                .ToArray();
            if (duplicateRules.Any())
            {
                var positionNames = _positionReadModel.GetPositionNames(duplicateRules.Select(x => x.PositionId)
                                                                                      .Concat(duplicateRules.Select(x => x.PositionDeniedId)));

                throw new EntityIsNotUniqueException(typeof(DeniedPosition),
                                                     string.Format(BLResources.DuplicateDeniedPositionRulesAreFound,
                                                                   string.Join(",",
                                                                               duplicateRules.Select(x =>
                                                                                                     string.Format("({0}, {1})",
                                                                                                                   positionNames[x.PositionId],
                                                                                                                   positionNames[x.PositionDeniedId])))));
            }

            var rulesWithoutSymmetric = deniedPositions.Where(deniedPosition => !deniedPositions.Any(x =>
                                                                                                     x.PositionId == deniedPosition.PositionDeniedId &&
                                                                                                     x.PositionDeniedId == deniedPosition.PositionId &&
                                                                                                     x.ObjectBindingType == deniedPosition.ObjectBindingType))
                                                       .Take(RulesToShowLimiter)
                                                       .ToArray();

            if (rulesWithoutSymmetric.Any())
            {
                var positionNames =
                    _positionReadModel.GetPositionNames(rulesWithoutSymmetric.Select(x => x.PositionId)
                                                                             .Concat(rulesWithoutSymmetric.Select(x => x.PositionDeniedId)));

                throw new SymmetricDeniedPositionRuleIsMissingException(string.Format(BLResources.SymmetricDeniedPositionRuleIsMissing,
                                                                                      string.Join(",",
                                                                                                  rulesWithoutSymmetric.Select(x =>
                                                                                                                               string.Format("({0}, {1})",
                                                                                                                                             positionNames[x.PositionId],
                                                                                                                                             positionNames[x.PositionDeniedId])))));
            }
        }

        private class DeniedPositionComparer : EqualityComparer<DeniedPosition>
        {
            public override bool Equals(DeniedPosition x, DeniedPosition y)
            {
                if (x == null || y == null)
                {
                    return x == null && y == null;
                }

                return x.PositionId == y.PositionId &&
                       x.PositionDeniedId == y.PositionDeniedId &&
                       x.ObjectBindingType == y.ObjectBindingType;
            }

            public override int GetHashCode(DeniedPosition obj)
            {
                return obj == null ? 0 : (obj.PositionId.GetHashCode() * 32) ^ obj.PositionDeniedId.GetHashCode() ^ (int)obj.ObjectBindingType;
            }
        }
    }
}
