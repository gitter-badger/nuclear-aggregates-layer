using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel
{
    public static partial class ClientSpecs
    {
        public static class Clients
        {
            public static class Find
            {
                public static FindSpecification<Client> ByMainFirm(long firmId)
                {
                    return new FindSpecification<Client>(x => x.MainFirmId == firmId);
                }

                public static FindSpecification<Client> ByFirm(long firmId)
                {
                    return new FindSpecification<Client>(x => x.Firms.Any(y => y.Id == firmId));
                }
            }

            public static class Select
            {
            }
        }
    }
}