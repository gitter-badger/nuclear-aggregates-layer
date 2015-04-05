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
