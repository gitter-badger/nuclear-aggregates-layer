using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel
{
    public static partial class ClientSpecs
    {
        public static class ClientLinks
        {
            public static class Find
            {
                public static FindSpecification<ClientLink> ByChildClientId(long childClientId)
                {
                    return new FindSpecification<ClientLink>(x => x.ChildClientId == childClientId);
                }

                public static FindSpecification<ClientLink> WhereMasterClientIsAdvertisingAgency()
                {
                    return new FindSpecification<ClientLink>(x => x.MasterClient.IsAdvertisingAgency);
                }
            }
        }
    }
}
