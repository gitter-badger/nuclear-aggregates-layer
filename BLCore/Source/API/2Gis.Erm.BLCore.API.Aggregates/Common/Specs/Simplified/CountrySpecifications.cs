using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
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