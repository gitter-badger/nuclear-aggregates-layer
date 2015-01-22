using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old
{
    public sealed class EditAccessSharingRequest : Request
    {
        public IEntityType EntityType { get; set; }
        public long EntityId { get; set; }
        public long EntityOwnerId { get; set; }
        public Guid EntityReplicationCode { get; set; }
        public IEnumerable<SharingDescriptor> AccessSharings { get; set; }
    }
}
