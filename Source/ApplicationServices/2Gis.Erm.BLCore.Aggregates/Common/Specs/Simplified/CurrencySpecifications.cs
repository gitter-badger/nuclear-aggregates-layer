﻿using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified
{
    public class CurrencySpecifications
    {
        public static class Find
        {
            public static FindSpecification<Currency> ById(long id)
            {
                return new FindSpecification<Currency>(x => x.Id == id);
            }

            public static FindSpecification<CurrencyRate> CurrencyRateById(long id)
            {
                return new FindSpecification<CurrencyRate>(x => x.Id == id);
            }
        }
    }
}
