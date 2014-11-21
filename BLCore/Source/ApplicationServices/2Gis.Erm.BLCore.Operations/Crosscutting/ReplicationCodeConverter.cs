using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    // FIXME {all, 21.10.2013}: нужно избавиться от использования DAL напрямую, скорее всего реализовать нужно как operation specific aggregate service
    public sealed class ReplicationCodeConverter : IReplicationCodeConverter
    {
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;

        public ReplicationCodeConverter(IFinder finder, ISecureFinder secureFinder)
        {
            _finder = finder;
            _secureFinder = secureFinder;
        }

        public long ConvertToEntityId(EntityName entityName, Guid replicationCode)
        {
            IQueryable<IReplicableEntity> unsecureQuery;
            IQueryable<IReplicableEntity> secureQuery;
            GetQueries(entityName.AsEntityType(), out unsecureQuery, out secureQuery);
            
            var entityId = unsecureQuery.Where(x => x.ReplicationCode == replicationCode).Select(x => (long?)x.Id).SingleOrDefault();
            if (entityId == null)
            {
                throw new ArgumentException(BLResources.CannotFindEntityByReplicationCode, "replicationCode");
            }

            var userCanRead = secureQuery.Any(x => x.ReplicationCode == replicationCode);
            if (!userCanRead)
            {
                throw new ArgumentException(BLResources.CurrentUserHasNoReadEntityPermission, "replicationCode");
            }

            return entityId.Value;
        }

        public Guid ConvertToReplicationCode(EntityName entityName, long entityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<long> ConvertToEntityIds(EntityName entityName, IEnumerable<Guid> replicationCodes)
        {
            IQueryable<IReplicableEntity> unsecureQuery;
            IQueryable<IReplicableEntity> secureQuery;
            GetQueries(entityName.AsEntityType(), out unsecureQuery, out secureQuery);
            var entityIds = unsecureQuery
                .Where(x => replicationCodes.Contains(x.ReplicationCode))
                .Select(x => x.Id).ToArray();
            if (entityIds.Count() != replicationCodes.Count())
            {
                throw new ArgumentException("Some replication codes cannot be converted to entity identifiers", "replicationCodes");
            }

            return entityIds;
        }

        public IEnumerable<Guid> ConvertToReplicationCodes(EntityName entityName, IEnumerable<long> entityIds)
        {
            throw new NotImplementedException();
        }

        private void GetQueries(
            Type entityType,
            out IQueryable<IReplicableEntity> unsecureQueryService,
            out IQueryable<IReplicableEntity> secureQueryService)
        {
            unsecureQueryService = _finder.FindAll(entityType) as IQueryable<IReplicableEntity>;
            secureQueryService = _secureFinder.FindAll(entityType) as IQueryable<IReplicableEntity>;
            if (unsecureQueryService == null || secureQueryService == null)
            {
                throw new ArgumentException("Entity is not replicable", "entityType");
            }
        }
    }
}