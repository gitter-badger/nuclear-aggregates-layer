using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class UserAuthorizationDoc
    {
        public ICollection<OperationPermission> Permissions { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}