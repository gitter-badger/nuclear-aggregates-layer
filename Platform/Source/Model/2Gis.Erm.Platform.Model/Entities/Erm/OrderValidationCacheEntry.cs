using System;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class OrderValidationCacheEntry :
        IEntity
    {
        public long OrderId { get; set; }
        public int ValidatorId { get; set; }
        public byte[] ValidVersion { get; set; }
        public Guid OperationId { get; set; }
    }
}