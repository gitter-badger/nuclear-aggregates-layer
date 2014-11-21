using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.PostIntegrationActivities
{
    public sealed class SyncFirmAddressesRequest : Request
    {
        public string Language { get; set; }
        public IEnumerable<FirmAddress> FirmAddresses { get; set; }
    }
}
