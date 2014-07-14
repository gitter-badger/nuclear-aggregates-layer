using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Docs
{
    public sealed class OperationPermission
    {
        public string Operation { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}