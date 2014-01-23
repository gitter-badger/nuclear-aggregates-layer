using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.API.Operations.Documents
{
    public sealed class UserDoc
    {
        public string Name { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
