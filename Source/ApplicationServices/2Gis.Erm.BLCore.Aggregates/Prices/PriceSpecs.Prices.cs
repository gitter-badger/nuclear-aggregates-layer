using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public static class PriceSpecs
    {
        public static class Prices
        {
            public static class Find
            {
                public static FindSpecification<Price> Linked()
                {
                    return
                        new FindSpecification<Price>(
                            x => x.PricePositions
                                    .Any(pp => !pp.IsDeleted 
                                            && pp.OrderPositions
                                                    .Any(op => !op.IsDeleted && op.Order.IsActive && !op.Order.IsDeleted)));
                }
            }
        }
    }
}
