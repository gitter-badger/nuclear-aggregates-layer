using DoubleGis.Erm.Platform.DAL.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified
{
    public class PlatformSpecifications
    {
        public static class Find
        {
            public static FindSpecification<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform> ById(long id)
            {
                return new FindSpecification<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>(x => x.Id == id);
            }
        }
    }
}
