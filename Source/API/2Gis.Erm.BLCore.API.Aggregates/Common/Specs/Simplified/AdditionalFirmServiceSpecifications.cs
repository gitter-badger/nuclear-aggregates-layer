using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
{
    public static class AdditionalFirmServiceSpecifications
    {
        public static class Find
        {
            public static FindSpecification<AdditionalFirmService> ById(long id)
            {
                return new FindSpecification<AdditionalFirmService>(x => x.Id == id);
            }
        }
    }
}