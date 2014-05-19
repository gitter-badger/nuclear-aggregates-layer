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

                public static FindSpecification<BranchOfficeOrganizationUnit> BelongsToOrganizationUnit(long organizationUnitId)
                {
                    return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.OrganizationUnitId == organizationUnitId);
                }
            }
        }
    }
}