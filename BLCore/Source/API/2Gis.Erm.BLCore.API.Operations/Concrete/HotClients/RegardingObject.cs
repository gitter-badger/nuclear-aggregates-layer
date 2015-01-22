using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients
{
    public sealed class RegardingObject
    {
        public IEntityType EntityName { get; set; }
        public long EntityId { get; set; }
    }
}
