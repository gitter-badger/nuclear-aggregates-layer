using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Tests.Unit.ApplicationServices.Aggregates.Clients
{
    internal sealed class DenormalizedLinksTestData
    {
        public List<DenormalizedClientLink> DenormalizedClientLinks { get; set; }

        public List<DenormalizedClientLink> ExpectedLinks { get; set; }

        public List<DenormalizedClientLink> Result { get; set; }

        public long MasterClientId { get; set; }

        public long ChildClientId { get; set; }
    }
}