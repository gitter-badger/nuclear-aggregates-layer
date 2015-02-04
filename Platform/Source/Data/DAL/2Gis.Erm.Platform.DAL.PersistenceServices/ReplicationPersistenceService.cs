using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    public sealed class ReplicationPersistenceService : IReplicationPersistenceService
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IDatabaseCaller _databaseCaller;
        private readonly ICommonLog _logger;
        private readonly IMsCrmReplicationMetadataProvider _msCrmReplicationMetadataProvider;

        public ReplicationPersistenceService(
            IMsCrmSettings msCrmSettings,
            IDatabaseCaller databaseCaller,
            ICommonLog logger,
            IMsCrmReplicationMetadataProvider msCrmReplicationMetadataProvider)
        {
            _msCrmSettings = msCrmSettings;
            _databaseCaller = databaseCaller;
            _logger = logger;
            _msCrmReplicationMetadataProvider = msCrmReplicationMetadataProvider;
        }

        public void ReplicateToMsCrm(Type entityType, IReadOnlyCollection<long> ids, TimeSpan timeout, out IReadOnlyCollection<long> notReplicated)
        {
            if (!_msCrmSettings.EnableReplication)
            {
                _logger.WarnFormat("Replication to MsCRM disabled in config. Do nothing ...");
                notReplicated = new long[0];
                return;
            }

            EntityReplicationInfo entityReplicationInfo;
            if (!_msCrmReplicationMetadataProvider.TryGetAsyncMetadata(entityType, ReplicationMode.Batch, out entityReplicationInfo))
            {
                throw new InvalidOperationException("Can't find replication metadata for specified entity type " + entityType.FullName);
            }

            switch (entityReplicationInfo.ReplicationMode)
            {
                case ReplicationMode.Single:
                {
                    ReplicateSingle(entityReplicationInfo.SchemaQualifiedStoredProcedureName, timeout, ids, out notReplicated);
                    break;
                }
                case ReplicationMode.Batch:
                {
                    ReplicateBatch(entityReplicationInfo.SchemaQualifiedStoredProcedureName, timeout, ids, out notReplicated);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void ReplicateSingle(string procedureName, TimeSpan timeout, IReadOnlyCollection<long> ids, out IReadOnlyCollection<long> notReplicated)
        {
            var failed = new List<long>();

            foreach (var id in ids)
            {
                try
                {
                    _databaseCaller.ExecuteProcedure(procedureName, timeout, new Tuple<string, object>("Id", id));
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormat(ex, "Can't replicate entity with id {0} using procedure {1}", id, procedureName);
                    failed.Add(id);
                }
            }

            notReplicated = failed;
        }

        private void ReplicateBatch(string procedureName, TimeSpan timeout, IReadOnlyCollection<long> ids, out IReadOnlyCollection<long> notReplicated)
        {
            notReplicated = null;

            try
            {
                _databaseCaller.ExecuteProcedure(procedureName, timeout, new Tuple<string, object>("Ids", ids.ToIdsContainer()));
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Can't replicate entities batch using procedure {0}", procedureName);
                notReplicated = new List<long>(ids);
            }
        }
    }
}
