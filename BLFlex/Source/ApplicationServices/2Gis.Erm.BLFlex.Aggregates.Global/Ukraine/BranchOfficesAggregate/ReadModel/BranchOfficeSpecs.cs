using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.BranchOfficesAggregate.ReadModel
{
    public static class BranchOfficeSpecs
    {
        public static class Find
        {
            public static FindSpecification<BranchOffice> DuplicatesByEgrpou(long entityId, string egrpou)
            {
                return new FindSpecification<BranchOffice>(x => x.Id != entityId && x.Inn == egrpou);
            }
        }
    }
}
