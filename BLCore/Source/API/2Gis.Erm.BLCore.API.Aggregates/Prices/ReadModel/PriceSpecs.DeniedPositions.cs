using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel
{
    public static partial class PriceSpecs
    {
        public static class DeniedPositions
        {
            public static class Find
            {
                public static FindSpecification<DeniedPosition> ByPrice(long priceId)
                {
                    return new FindSpecification<DeniedPosition>(x => x.PriceId == priceId);
                }

                public static FindSpecification<DeniedPosition> ByPosition(long positionId)
                {
                    return new FindSpecification<DeniedPosition>(x => x.PositionId == positionId);
                }

                public static FindSpecification<DeniedPosition> ByPositionDenied(long positionDeniedId)
                {
                    return new FindSpecification<DeniedPosition>(x => x.PositionDeniedId == positionDeniedId);
                }

                public static FindSpecification<DeniedPosition> ByPositions(long positionId, long positionDeniedId)
                {
                    return new FindSpecification<DeniedPosition>(x => x.PositionId == positionId && x.PositionDeniedId == positionDeniedId);
                }

                public static FindSpecification<DeniedPosition> ByObjectBindingType(ObjectBindingType objectBindingType)
                {
                    return new FindSpecification<DeniedPosition>(x => x.ObjectBindingType == objectBindingType);
                }
            }
        }
    }
}