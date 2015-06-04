using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary
{
    public static class OrganizationUnitSpecs
    {
        public static class OrganizationUnits
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
            public static SelectSpecification<OrganizationUnit, decimal> VatRate()
            {
                return new SelectSpecification<OrganizationUnit, decimal>(
                    x => x.BranchOfficeOrganizationUnits
                          .FirstOrDefault(unit => unit.IsPrimaryForRegionalSales).BranchOffice.BargainType.VatRate);
            }
        }
    }

        public static class UserOrganizationUnits
        {
            public static class Find
            {
                public static FindSpecification<UserOrganizationUnit> ByUser(long userId)
                {
                    return new FindSpecification<UserOrganizationUnit>(x => x.UserId == userId);
                }
            }
        }
    }
}