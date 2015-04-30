using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel
{
    public static partial class FirmSpecs
    {
        public static class FirmContacts
        {
            public static class Find
            {
                public static FindSpecification<FirmContact> ByFirmAddresses(IEnumerable<long> firmAddressIds)
                {
                    return new FindSpecification<FirmContact>(x => x.FirmAddressId.HasValue && firmAddressIds.Contains(x.FirmAddressId.Value));
                }

                public static FindSpecification<FirmContact> ByDepCards(IEnumerable<long> depCardIds)
                {
                    return new FindSpecification<FirmContact>(x => x.CardId.HasValue && depCardIds.Contains(x.CardId.Value));
                }
            }
        }
    }
}