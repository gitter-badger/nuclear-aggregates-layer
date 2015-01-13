using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public static class CategorySpecs
    {
        public static class CategoryOrganizationUnits
        {
            public static class Find
            {
                public static FindSpecification<CategoryOrganizationUnit> ForOrganizationUnit(long organizationUnitId)
                {
                    return new FindSpecification<CategoryOrganizationUnit>(x => x.OrganizationUnitId == organizationUnitId);
                }
            }
        }

        public static class CategoryFirmAddresses
        {
            public static class Find
            {
                public static FindSpecification<CategoryFirmAddress> ByFirmAddresses(IEnumerable<long> addressIds)
                {
                    return new FindSpecification<CategoryFirmAddress>(x => addressIds.Contains(x.FirmAddressId));
                }
            }
        }

        public static class SalesModelCategoryRestrictions
        {
            public static class Find
            {
                public static FindSpecification<SalesModelCategoryRestriction> BySalesModelAndOrganizationUnit(SalesModel model, long organizationUnitId)
                {
                    return new FindSpecification<SalesModelCategoryRestriction>(x => x.SalesModel == model && x.Project.OrganizationUnitId == organizationUnitId);
                }

                public static FindSpecification<SalesModelCategoryRestriction> ByProject(long projectId)
                {
                    return new FindSpecification<SalesModelCategoryRestriction>(x => x.ProjectId == projectId);
                }
            }
        }
    }
}