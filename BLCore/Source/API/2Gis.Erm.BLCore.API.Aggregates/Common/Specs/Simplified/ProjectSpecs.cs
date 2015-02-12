using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
{
    public static class ProjectSpecs
    {
        public static class Find
        {
            public static FindSpecification<Project> ByOrganizationUnit(long organizationUnitId)
            {
                return new FindSpecification<Project>(x => x.OrganizationUnitId == organizationUnitId);
            }
        }
    }
}