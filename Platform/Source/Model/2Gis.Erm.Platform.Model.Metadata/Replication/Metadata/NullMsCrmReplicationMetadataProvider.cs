using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata
{
    public class NullMsCrmReplicationMetadataProvider : IMsCrmReplicationMetadataProvider
    {
        public bool TryGetMetadata(Type entityType, ReplicationMode preferredMode, out EntityReplicationInfo replicationInfo)
        {
            replicationInfo = null;
            return false;
        }

        public IEnumerable<Type> GetReplicationTypeSequence()
        {
            return Enumerable.Empty<Type>();
        }
    }
}