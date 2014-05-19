using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel
{
    public static partial class FirmSpecs
    {
        public static class Addresses
        {
            public static class Find
            {
                public static FindSpecification<FirmAddress> ActiveAddresses(long firmId)
                {
                    return new FindSpecification<FirmAddress>(address => address.IsActive && !address.IsDeleted && address.FirmId == firmId);
                }
            }
        }
    }
}