﻿using System.Collections.Generic;
using System.Linq;

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
                public static FindSpecification<FirmAddress> ActiveByFirmId(long firmId)
                {
                    return new FindSpecification<FirmAddress>(address => address.FirmId == firmId && (address.IsActive && !address.IsDeleted));
                }

                public static FindSpecification<FirmAddress> ActiveOrWithSalesByFirmId(long firmId)
                {
                    return new FindSpecification<FirmAddress>(address => address.FirmId == firmId && (address.IsActive && !address.IsDeleted || address.OrderPositionAdvertisements.Any()));
                }

                public static FindSpecification<FirmAddress> ByFirmIds(IEnumerable<long> firmIds)
                {
                    return new FindSpecification<FirmAddress>(address => firmIds.Contains(address.FirmId));
                }

                public static FindSpecification<FirmAddress> NotClosed()
                {
                    return new FindSpecification<FirmAddress>(address => !address.ClosedForAscertainment);
                }

                public static FindSpecification<FirmAddress> WithAddressCode()
                {
                    return new FindSpecification<FirmAddress>(address => address.AddressCode.HasValue);
                }
            }
        }
    }
}