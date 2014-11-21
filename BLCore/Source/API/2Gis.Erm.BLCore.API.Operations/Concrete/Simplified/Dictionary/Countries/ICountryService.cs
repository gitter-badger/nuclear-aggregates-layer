﻿using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Countries
{
    public interface ICountryService : ISimplifiedModelConsumer
    {
        void CreateOrUpdate(Country country);
    }
}
