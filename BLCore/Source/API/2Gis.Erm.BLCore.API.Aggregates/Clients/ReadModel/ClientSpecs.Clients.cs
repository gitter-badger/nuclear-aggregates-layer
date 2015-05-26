using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;
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

                public static FindSpecification<Client> ByMainFirms(IEnumerable<long> firmIds)
                {
                    return new FindSpecification<Client>(x => x.MainFirmId.HasValue && firmIds.Contains(x.MainFirmId.Value));
                }

                public static FindSpecification<Client> ByFirm(long firmId)
                {
                    return new FindSpecification<Client>(x => x.Firms.Any(y => y.Id == firmId));
                }

                public static FindSpecification<Client> ByContact(long contactId)
                {
                    return new FindSpecification<Client>(x => x.Contacts.Any(y => y.Id == contactId));
                }

                public static FindSpecification<Client> ByTerritory(long territoryId)
                {
                    return new FindSpecification<Client>(x => x.TerritoryId == territoryId);
                }

                public static IFindSpecification<Client> ByDeal(long dealId)
                {
                    return new FindSpecification<Client>(x => x.Deals.Any(y => y.Id == dealId));
                }
            }

            public static class Select
            {
            }
        }
    }
}