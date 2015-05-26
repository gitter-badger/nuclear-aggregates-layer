using System.Collections.Generic;
using System.Linq;


using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel
{
    public static partial class DealSpecs
    {
        public static class Deals
        {
            public static class Find
            {
                public static FindSpecification<Deal> ForClient(long clientId)
                {
                    return new FindSpecification<Deal>(x => x.ClientId == clientId);
                }

                public static FindSpecification<Deal> ByClientIds(IEnumerable<long?> clientAndChild)
                {
                    return new FindSpecification<Deal>(x => clientAndChild.Contains(x.ClientId));
                }

                public static FindSpecification<Deal> ByMainFirms(IEnumerable<long> mainFirmIds)
                {
                    return new FindSpecification<Deal>(x => x.MainFirmId.HasValue && mainFirmIds.Contains(x.MainFirmId.Value));
                }
            }
        }
    }
}