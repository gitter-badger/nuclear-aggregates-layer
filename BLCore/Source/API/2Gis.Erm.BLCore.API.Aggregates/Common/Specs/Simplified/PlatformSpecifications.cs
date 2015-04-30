using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
{
    public class PlatformSpecifications
    {
        public static class Find
        {
            public static FindSpecification<Platform.Model.Entities.Erm.Platform> ById(long id)
            {
                return new FindSpecification<Platform.Model.Entities.Erm.Platform>(x => x.Id == id);
            }
        }
    }
}