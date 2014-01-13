using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.DTO
{
    public sealed class ClientReplicationDto
    {
        public Guid ClientReplicationCode { get; set; }
        public IEnumerable<Guid> FirmReplicationCodes { get; set; }
    }
}
