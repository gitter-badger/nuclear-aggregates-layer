using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public static class DeniedPositionExtensions
    {
        public static bool IsSelfDenied(this DeniedPosition deniedPosition)
        {
            return deniedPosition.PositionId == deniedPosition.PositionDeniedId;
        }

        public static bool IsSymmetricTo(this DeniedPosition deniedPosition, DeniedPosition symmetricDeniedPosition)
        {
            return deniedPosition.PositionId == symmetricDeniedPosition.PositionDeniedId &&
                   deniedPosition.PositionDeniedId == symmetricDeniedPosition.PositionId &&
                   deniedPosition.PriceId == symmetricDeniedPosition.PriceId &&
                   deniedPosition.ObjectBindingType == symmetricDeniedPosition.ObjectBindingType;
        }

        public static IEnumerable<DeniedPosition> DistinctDeniedPositions(this IEnumerable<DeniedPosition> deniedPositions)
        {
            return deniedPositions.Distinct(new DeniedPositionComparer()).ToArray();
        }

        public static IEnumerable<DeniedPosition> PickDuplicates(this IEnumerable<DeniedPosition> deniedPositions)
        {
            return deniedPositions.GroupBy(x => new
                                                    {
                                                        x.PositionId,
                                                        x.PositionDeniedId,
                                                    })
                                  .Where(x => x.Count() > 1)
                                  .SelectMany(x => x)
                                  .ToArray();
        }

        public static IEnumerable<DeniedPosition> PickRulesWithoutSymmetricOnes(this IEnumerable<DeniedPosition> deniedPositions)
        {
            return deniedPositions.Where(deniedPosition => !deniedPositions.Any(x =>
                                                                                x.PositionId == deniedPosition.PositionDeniedId &&
                                                                                x.PositionDeniedId == deniedPosition.PositionId &&
                                                                                x.ObjectBindingType == deniedPosition.ObjectBindingType))
                                  .ToArray();
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
                       x.ObjectBindingType == y.ObjectBindingType &&
                       x.IsActive == y.IsActive &&
                       x.IsDeleted == y.IsDeleted;
            }

            public override int GetHashCode(DeniedPosition obj)
            {
                return obj == null ? 0 : (obj.PositionId.GetHashCode() * 32) ^ obj.PositionDeniedId.GetHashCode() ^ (int)obj.ObjectBindingType;
            }
        }
    }
}
