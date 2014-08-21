using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class DocumentAuthorization
    {
        public ICollection<string> Operations { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}