using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices
{
    public static class BranchOfficeSpecifications
    {
        public static class Find
        {
            public static FindSpecification<BranchOffice> ById(long id)
            {
                return new FindSpecification<BranchOffice>(x => x.Id == id);
            }

            public static FindSpecification<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnitById(long id)
            {
                return new FindSpecification<BranchOfficeOrganizationUnit>(x => x.Id == id);
            }

            public static FindSpecification<ContributionType> ContributionTypeById(long id)
            {
                return new FindSpecification<ContributionType>(x => x.Id == id);
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
