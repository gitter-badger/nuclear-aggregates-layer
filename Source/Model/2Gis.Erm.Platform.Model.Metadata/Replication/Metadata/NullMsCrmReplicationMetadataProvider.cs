using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata
{
    public class NullMsCrmReplicationMetadataProvider : IMsCrmReplicationMetadataProvider
    {
        public bool TryGetAsyncMetadata(Type entityType, ReplicationMode preferredMode, out EntityReplicationInfo replicationInfo)
        {
            replicationInfo = null;
            return false;
        }

        public bool TryGetSyncMetadata(Type entityType, out EntityReplicationInfo replicationInfo)
        {
            replicationInfo = null;
            return false;
        }

        public IEnumerable<Type> GetAsyncReplicationTypeSequence()
        {
            return Enumerable.Empty<Type>();
        }
    }
}