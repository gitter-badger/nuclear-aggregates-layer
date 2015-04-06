using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata
{
    public interface IMsCrmReplicationMetadataProvider
    {
        bool TryGetMetadata(Type entityType, ReplicationMode preferredMode, out EntityReplicationInfo replicationInfo);
        IEnumerable<Type> GetReplicationTypeSequence();
    }
}