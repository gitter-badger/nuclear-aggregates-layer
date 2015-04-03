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
    }
}
