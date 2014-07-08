using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    public sealed class ReplicationPersistenceService : IReplicationPersistenceService
    {
        private delegate void Replicator(string procedureName, int timeout, IEnumerable<long> ids, out IEnumerable<long> notReplicated);

        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IDatabaseCaller _databaseCaller;
        private readonly ICommonLog _logger;

        private readonly IReadOnlyDictionary<Type, Tuple<string, Replicator>> _replicatorsMap;

        public ReplicationPersistenceService(
            IMsCrmSettings msCrmSettings,
            IDatabaseCaller databaseCaller, 
            ICommonLog logger)
        {
            _msCrmSettings = msCrmSettings;
            _databaseCaller = databaseCaller;
            _logger = logger;

            var replicatorsMap =
                new Dictionary<Type, Tuple<string, Replicator>>()
                {
                    { typeof(Firm), new Tuple<string, Replicator>("BusinessDirectory.ReplicateFirms", ReplicateBatch) },
                    { typeof(FirmAddress), new Tuple<string, Replicator>("BusinessDirectory.ReplicateFirmAddresses", ReplicateBatch) },
                    { typeof(Territory), new Tuple<string, Replicator>("BusinessDirectory.ReplicateTerritories", ReplicateBatch) },
                    { typeof(Client), new Tuple<string, Replicator>("Billing.ReplicateClient", ReplicateSingle) },
                };

            _replicatorsMap = replicatorsMap;
        }

        public void ReplicateToMscrm(Type entityType, IEnumerable<long> ids, int timeout, out IEnumerable<long> notReplicated)
        {
            if (!_msCrmSettings.EnableReplication)
            {
                _logger.WarnFormatEx("Replication to MsCRM disableв шт config. Do nothing ...");
                notReplicated = Enumerable.Empty<long>();
                return;
            }

            Tuple<string, Replicator> replicatorInfo;
            if (!_replicatorsMap.TryGetValue(entityType, out replicatorInfo))
            {
                throw new InvalidOperationException("Can't find replicator for specified entity type " + entityType.FullName);
            }

            replicatorInfo.Item2(replicatorInfo.Item1, timeout, ids, out notReplicated);
        }

        private void ReplicateSingle(string procedureName, int timeout, IEnumerable<long> ids, out IEnumerable<long> notReplicated)
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
                    _logger.ErrorFormatEx(ex, "Can't replicate entity with id {0} using procedure {1}", id, procedureName);
                    failed.Add(id);
                }
            }

            notReplicated = failed;
        }

        private void ReplicateBatch(string procedureName, int timeout, IEnumerable<long> ids, out IEnumerable<long> notReplicated)
        {
            notReplicated = null;

            try
            {
                _databaseCaller.ExecuteProcedure(procedureName, timeout, new Tuple<string, object>("Ids", ids.ToIdsContainer()));
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Can't replicate entities batch using procedure {0}", procedureName);
                notReplicated = new List<long>(ids);
            }
        }
    }
}
