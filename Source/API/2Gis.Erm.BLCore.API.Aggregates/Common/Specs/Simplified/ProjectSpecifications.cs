using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
{
    public static class ProjectSpecs
    {
        public static class Find
        {
            public static FindSpecification<Project> ByCode(long code)
            {
                return new FindSpecification<Project>(x => x.Code == code);
            }
        }
    }
}