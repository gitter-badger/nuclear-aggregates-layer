using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public static class OrganizationUnitSpecs
    {
        public static class Find
        {
            public static FindSpecification<OrganizationUnit> ByDgppId(long organizationUnitDgppId)
            {
                return new FindSpecification<OrganizationUnit>(x => x.DgppId == organizationUnitDgppId);
            }

            public static FindSpecification<OrganizationUnit> ByDgppIds(IEnumerable<int> organizationUnitDgppIds)
            {
                return new FindSpecification<OrganizationUnit>(x => x.DgppId.HasValue && organizationUnitDgppIds.Contains(x.DgppId.Value));
            }

            public static FindSpecification<OrganizationUnit> DestOrganizationUnitByOrder(long orderId)
            {
                return new FindSpecification<OrganizationUnit>(x => x.OrdersByDestination.Any(y => y.IsActive && !y.IsDeleted && y.Id == orderId));
            }
        }

        public static class Select
        {
            public static ISelectSpecification<OrganizationUnit, decimal> VatRate()
            {
                return new SelectSpecification<OrganizationUnit, decimal>(
                    x => x.BranchOfficeOrganizationUnits
                          .FirstOrDefault(unit => unit.IsPrimaryForRegionalSales).BranchOffice.BargainType.VatRate);
            }
        }
    }
}