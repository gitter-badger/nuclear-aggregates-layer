using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class ReplicationQueue
    {
        public string DocumentType { get; set; }
        public ICollection<IndexSettings> IndexesSettings { get; set; }
        public string Progress { get; set; }

        public sealed class IndexSettings
        {
            public string IndexName { get; set; }
            public int? NumberOfReplicas { get; set; }
            public string RefreshInterval { get; set; }
        }
    }
}
