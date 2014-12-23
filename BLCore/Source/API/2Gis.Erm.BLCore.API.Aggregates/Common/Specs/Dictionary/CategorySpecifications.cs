using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public static class CategorySpecifications
    {
        public static class CategoryOrganizationUnits
        {
            public static class Find
            {
                public static FindSpecification<CategoryOrganizationUnit> ForOrganizationUnit(long organizationUnitId)
                {
                    return new FindSpecification<CategoryOrganizationUnit>(x => x.OrganizationUnitId == organizationUnitId);
                }

                public static FindSpecification<CategoryOrganizationUnit> ForCategories(IEnumerable<long> categoryIds)
                {
                    return new FindSpecification<CategoryOrganizationUnit>(x => categoryIds.Contains(x.CategoryId));
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
    }
}