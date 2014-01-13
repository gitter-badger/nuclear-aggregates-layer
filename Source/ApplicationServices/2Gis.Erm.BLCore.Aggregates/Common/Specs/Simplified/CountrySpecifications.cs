using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified
{
    public class CountrySpecifications
    {
        public static class Find
        {
            public static FindSpecification<Country> ById(long id)
            {
                return new FindSpecification<Country>(x => x.Id == id);
            }
        }
    }
}
