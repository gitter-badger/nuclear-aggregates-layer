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