using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel
{
    public static class BranchOfficeSpecifications
    {
        public static class Find
        {
            public static FindSpecification<BranchOffice> ById(long id)
            {
                return new FindSpecification<BranchOffice>(x => x.Id == id);
            }

            public static FindSpecification<ContributionType> ContributionTypeById(long id)
            {
                return new FindSpecification<ContributionType>(x => x.Id == id);
            }
        }
    }
}
