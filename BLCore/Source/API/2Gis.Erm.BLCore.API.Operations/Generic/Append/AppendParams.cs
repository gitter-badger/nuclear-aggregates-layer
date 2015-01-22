using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Append
{
    public sealed class AppendParams
    {
        public long? ParentId { get; set; }
        public IEntityType ParentType { get; set; }
        public long? AppendedId { get; set; }
        public IEntityType AppendedType { get; set; }
    }
}
