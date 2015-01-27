using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class OrderPositions
        {
            public static class Find
            {
                public static FindSpecification<OrderPosition> PlannedProvision()
                {
                    return new FindSpecification<OrderPosition>(x => x.PricePosition.Position.AccountingMethodEnum == PositionAccountingMethod.PlannedProvision);
                } 
            }
        }
    }
}