﻿using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus
{
    public sealed class ImportFlowGeographyRequest : Request
    {
        public string RegionalTerritoryLocaleSpecificWord { get; set; }
    }
}
