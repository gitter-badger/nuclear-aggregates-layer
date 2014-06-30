using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Docs
{
    public sealed class UserDoc : IDoc
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<OperationPermission> Permissions { get; set; }
    }
}