using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel
{
    public class BranchOfficeSpecs
    {
        public static class BranchOfficeOrganizationUnits
        {
            public static class Find
            {
                public static FindSpecification<BranchOfficeOrganizationUnit> BySyncCode1C(string syncCode1C)
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => !x.IsDeleted && x.IsActive && x.SyncCode1C == syncCode1C);
                }

                public static FindSpecification<BranchOfficeOrganizationUnit> PrimaryBranchOfficeOrganizationUnit()
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.IsPrimary &&
                                                                                    x.OrganizationUnit.IsActive && !x.OrganizationUnit.IsDeleted &&
                                                                                    x.BranchOffice.IsActive && !x.BranchOffice.IsDeleted);
                }

                public static FindSpecification<BranchOfficeOrganizationUnit> Primary()
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.IsPrimary);
                }

                public static IFindSpecification<BranchOfficeOrganizationUnit> PrimaryForRegionalSalesOfOrganizationUnit(long organizationUnitId)
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.IsPrimaryForRegionalSales &&
                                                                                    x.IsActive && !x.IsDeleted &&
                                                                                    x.OrganizationUnitId == organizationUnitId);
                }

                public static FindSpecification<BranchOfficeOrganizationUnit> ByOrganizationUnit(long organizationUnitId)
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.OrganizationUnitId == organizationUnitId);
                }

                public static IFindSpecification<BranchOfficeOrganizationUnit> ByBranchOffice(long branchOfficeId)
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.BranchOfficeId == branchOfficeId);
                }

                public static FindSpecification<BranchOfficeOrganizationUnit> ByBranchOffice(IEnumerable<long> branchOfficeIds)
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => branchOfficeIds.Contains(x.BranchOfficeId));
                }

                public static IFindSpecification<BranchOfficeOrganizationUnit> ByOrderId(long orderId)
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.Orders.Any(order => order.Id == orderId));
                }

                public static FindSpecification<BranchOfficeOrganizationUnit> DuplicatesByOrganizationUnitAndBranchOffice(long organizationUnitId,
                                                                                                                         long branchOfficeId,
                                                                                                                         long branchOfficeOrganizationUnitId)
                {
                    return
                        new FindSpecification<BranchOfficeOrganizationUnit>(
                            x => x.OrganizationUnitId == organizationUnitId && x.BranchOfficeId == branchOfficeId && x.Id != branchOfficeOrganizationUnitId);
                }
            }

            public static class Select
            {
                public static ISelectSpecification<BranchOfficeOrganizationUnit, BranchOfficeOrganizationUnitNamesDto> BranchOfficeAndOrganizationUnitNames()
                {
                    return new SelectSpecification<BranchOfficeOrganizationUnit, BranchOfficeOrganizationUnitNamesDto>(
                        x => new BranchOfficeOrganizationUnitNamesDto
                            {
                                OrganizationUnitName = x.OrganizationUnit.Name,
                                BranchOfficeName = x.BranchOffice.Name,
                                IsActive = x.IsActive
                            });
                }
            }
        }

        public static class BranchOffices
        {
            public static class Find
            {
                public static FindSpecification<BranchOffice> DuplicatesByInn(long entityId, string inn)
                {
                    return new FindSpecification<BranchOffice>(x => x.Id != entityId && x.Inn == inn);
                }
            }
        }
    }
}