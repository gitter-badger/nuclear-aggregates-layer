using System;

namespace DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata
{
    public class EntityReplicationInfo
    {
        private readonly Type _entityType;
        private readonly string _schemaQualifiedStoredProcedureName;
        private readonly string _storedProcedureName;
        private readonly ReplicationMode _replicationMode;

        internal EntityReplicationInfo(Type entityType, string schemaName, string storedProcedureName, ReplicationMode replicationMode)
        {
            _entityType = entityType;
            _storedProcedureName = storedProcedureName;
            _schemaQualifiedStoredProcedureName = string.Format("{0}.{1}", schemaName, storedProcedureName);
            _replicationMode = replicationMode;
        }

        public Type EntityType
        {
            get { return _entityType; }
        }

        public string StoredProcedureName
        {
            get { return _storedProcedureName; }
        }

        public string SchemaQualifiedStoredProcedureName
        {
            get { return _schemaQualifiedStoredProcedureName; }
        }

        public ReplicationMode ReplicationMode
        {
            get { return _replicationMode; }
        }
    }
}