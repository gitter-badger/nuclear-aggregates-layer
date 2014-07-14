using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Docs
{
    public sealed class DocumentAuthorization
    {
        public ICollection<string> Operations { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}