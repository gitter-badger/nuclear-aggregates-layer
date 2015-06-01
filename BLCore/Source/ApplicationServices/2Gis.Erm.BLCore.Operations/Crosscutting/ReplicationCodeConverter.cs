using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects.Integration;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    // FIXME {all, 21.10.2013}: нужно избавиться от использования DAL напрямую, скорее всего реализовать нужно как operation specific aggregate service
    public sealed class ReplicationCodeConverter : IReplicationCodeConverter
    {
        private readonly IQuery _query;
        private readonly ISecureQuery _secureQuery;

        public ReplicationCodeConverter(IQuery query, ISecureQuery secureQuery)
        {
            _query = query;
            _secureQuery = secureQuery;
        }

        public long ConvertToEntityId(IEntityType entityName, Guid replicationCode)
        {
            return LookupEntityId(entityName, replicationCode);
        }

        public Guid ConvertToReplicationCode(IEntityType entityName, long entityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<long> ConvertToEntityIds(IEntityType entityName, IEnumerable<Guid> replicationCodes)
        {
            var codes = replicationCodes.ToList();
            var entityIds = LookupEntities(_query, entityName, codes).Select(x => x.Id).ToList();
            if (entityIds.Count != codes.Count)
            {
                throw new ArgumentException("Some replication codes cannot be converted to entity identifiers", "replicationCodes");
            }

            return entityIds;
        }

        public IEnumerable<ErmEntityInfo> ConvertToEntityIds(IEnumerable<CrmEntityInfo> crmEntities)
        {
            var crmEntityInfos = crmEntities as IList<CrmEntityInfo> ?? crmEntities.ToList();
            var list = crmEntityInfos.GroupBy(p => p.EntityName, p => p.Id, (key, value) => new { EntityName = key, replicationCodes = value.ToList() });

            var resultList = new List<ErmEntityInfo>();
            foreach (var entityType in list)
            {
                var type = entityType;
                var entityIds = LookupEntities(_query, entityType.EntityName, entityType.replicationCodes)
                    .Select(x => new ErmEntityInfo { EntityName = type.EntityName, Id = x.Id });                    
                resultList.AddRange(entityIds);
            }

            if (resultList.Count != crmEntityInfos.Count())
            {
                throw new ArgumentException("Some replication codes cannot be converted to entity identifiers", "replicationCodes");
            } 

            return resultList;
        }

        public IEnumerable<Guid> ConvertToReplicationCodes(IEntityType entityName, IEnumerable<long> entityIds)
        {
            throw new NotImplementedException();
        }

        private long LookupEntityId(IEntityType entityName, Guid replicationCode)
        {
            var entity = LookupEntity(_query, entityName, replicationCode);
            if (entity == null)
            {
                throw new ArgumentException(BLResources.CannotFindEntityByReplicationCode, "replicationCode");
            }

            var userCannotRead = LookupEntity(_secureQuery, entityName, replicationCode) == null;
            if (userCannotRead)
            {
                throw new ArgumentException(BLResources.CurrentUserHasNoReadEntityPermission, "replicationCode");
            }

            return entity.Id;
        }

        private static IReplicableEntity LookupEntity(IQuery finder, IEntityType entityName, Guid replicationCode)
        {
            var findOneMethodInfo = finder.GetType().GetMethods().First(x => x.Name == "FindOne");
            if (findOneMethodInfo == null)
            {
                throw new ArgumentException("The finder does not have required method.", "finder");
            }

            var entityType = entityName.AsEntityType();

            var findSpecExpression = Expression.Call(
                ByReplicationCodeMethodInfo.MakeGenericMethod(entityType),
                Expression.Constant(replicationCode));

            var findOneExpression = Expression.Call(
                Expression.Constant(finder),
                findOneMethodInfo.MakeGenericMethod(entityType),
                new Expression[] { findSpecExpression });

            var queryLambda = Expression.Lambda<Func<IReplicableEntity>>(findOneExpression).Compile();

            return queryLambda();
        }

        private static IEnumerable<IReplicableEntity> LookupEntities(IQuery finder, IEntityType entityName, IEnumerable<Guid> replicationCodes)
        {
            var findManyMethodInfo = finder.GetType().GetMethods().First(x => x.Name == "FindMany");
            if (findManyMethodInfo == null)
            {
                throw new ArgumentException("The finder does not have required method.", "finder");
            }

            var entityType = entityName.AsEntityType();

            var findSpecExpression = Expression.Call(
                ByReplicationCodesMethodInfo.MakeGenericMethod(entityType),
                Expression.Constant(replicationCodes));

            var findManyExpression = Expression.Call(
                Expression.Constant(finder),
                findManyMethodInfo.MakeGenericMethod(entityType),
                new Expression[] { findSpecExpression });

            var queryLambda = Expression.Lambda<Func<IEnumerable<IReplicableEntity>>>(findManyExpression).Compile();

            return queryLambda();
        }

        private static readonly MethodInfo ByReplicationCodeMethodInfo = typeof(Specs.Find).GetMethods().First(x => x.Name == "ByReplicationCode");
        private static readonly MethodInfo ByReplicationCodesMethodInfo = typeof(Specs.Find).GetMethods().First(x => x.Name == "ByReplicationCodes");
    }
}