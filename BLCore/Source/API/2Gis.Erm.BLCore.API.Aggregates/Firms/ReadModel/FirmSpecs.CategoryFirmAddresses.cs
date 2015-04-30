using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel
{
    public static partial class FirmSpecs
    {
        public static class CategoryFirmAddresses
        {
            public static class Find
            {
                public static FindSpecification<CategoryFirmAddress> ByAddresses(IEnumerable<long> firmAddressIds)
                {
                    return new FindSpecification<CategoryFirmAddress>(x => firmAddressIds.Contains(x.FirmAddressId));
                }
            }
        }
    }
}