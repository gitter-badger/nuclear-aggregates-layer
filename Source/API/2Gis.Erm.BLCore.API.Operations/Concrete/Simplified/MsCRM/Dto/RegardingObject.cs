using System;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto
{
    public sealed class RegardingObject
    {
        public EntityName EntityName { get; set; }
        public long EntityId { get; set; }
        public Guid ReplicationCode { get; set; }
    }
}
