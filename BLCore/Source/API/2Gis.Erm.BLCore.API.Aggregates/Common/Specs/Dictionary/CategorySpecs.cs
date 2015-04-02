using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public static class CategorySpecs
    {
        public static class Categories
        {
            public static class Find
            {
                public static FindSpecification<Category> ActiveCategoryForSalesModelInOrganizationUnit(SalesModel salesModel, long organizationUnitId)
                {
                    return Platform.DAL.Specifications.Specs.Find.ActiveAndNotDeleted<Category>() &&
                           ForOrganizationUnit(organizationUnitId) &&

                           // Ограничение по моделям продаж действует только для рубрик 3-го уровня
                           (!ByLevel(3) || RestrictedBySalesModelAndOrganizationUnit(salesModel, organizationUnitId));
                    }

                private static FindSpecification<Category> ForOrganizationUnit(long organizationUnitId)
                    {
                    return new FindSpecification<Category>(x => x.CategoryOrganizationUnits.Any(y => y.IsActive && !y.IsDeleted && y.OrganizationUnitId == organizationUnitId));
                }

                private static FindSpecification<Category> RestrictedBySalesModelAndOrganizationUnit(SalesModel model, long organizationUnitId)
                {
                    return new FindSpecification<Category>(x => x.SalesModelRestrictions.Any(y => y.SalesModel == model && y.Project.OrganizationUnitId == organizationUnitId));
                }

                private static FindSpecification<Category> ByLevel(int level)
                {
                    return new FindSpecification<Category>(x => x.Level == level);
                }
            }
        }

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
                    return new FindSpecification<CategoryOrganizationUnit>(link => categoryIds.Contains(link.CategoryId));
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
                public static FindSpecification<SalesModelCategoryRestriction> ByProject(long projectId)
                {
                    return new FindSpecification<SalesModelCategoryRestriction>(x => x.ProjectId == projectId);
                }
            }
        }
    }
}