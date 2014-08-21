using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public static class CategorySpecifications
    {
        public static class Find
        {
            public static FindSpecification<CategoryGroup> CategoryGroupById(long id)
            {
                return new FindSpecification<CategoryGroup>(x => x.Id == id);
            }

            public static FindSpecification<CategoryOrganizationUnit> CategoriesForOrganizationUnit(long organizationUnitId)
            {
                return new FindSpecification<CategoryOrganizationUnit>(x => x.OrganizationUnitId == organizationUnitId);
            }
        }
    }
}