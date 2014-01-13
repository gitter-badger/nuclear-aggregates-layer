using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices
{
    public class DeniedPositionSpecifications
    {
        public static class Find
        {
            public static FindSpecification<DeniedPosition> ById(long id)
            {
                return new FindSpecification<DeniedPosition>(x => x.Id == id);
            }
        }
    }
}
