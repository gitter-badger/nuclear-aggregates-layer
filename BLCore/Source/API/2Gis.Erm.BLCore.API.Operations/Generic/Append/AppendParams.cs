using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Append
{
    public sealed class AppendParams
    {
        public long? ParentId { get; set; }
        public EntityName ParentType { get; set; }
        public long? AppendedId { get; set; }
        public EntityName AppendedType { get; set; }
    }
}
