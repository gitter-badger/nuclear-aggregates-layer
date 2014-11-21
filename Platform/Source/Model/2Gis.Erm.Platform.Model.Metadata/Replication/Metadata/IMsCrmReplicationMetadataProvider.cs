using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata
{
    public interface IMsCrmReplicationMetadataProvider
    {
        bool TryGetAsyncMetadata(Type entityType, ReplicationMode preferredMode, out EntityReplicationInfo replicationInfo);
        bool TryGetSyncMetadata(Type entityType, out EntityReplicationInfo replicationInfo);
        IEnumerable<Type> GetAsyncReplicationTypeSequence();
    }
}