﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
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
            return LookupEntityId(entityName, replicationCode);
        }

        public Guid ConvertToReplicationCode(EntityName entityName, long entityId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EntityInfo> ConvertToEntityIds(IEnumerable<EntityName> entityName, IEnumerable<Guid> replicationCodes)
        {            
            var list = entityName.Zip(replicationCodes, (k, v) => new Tuple<EntityName, Guid>(k, v))
                                 .GroupBy(p => p.Item1, p => p.Item2, (key, value) => new { EntityName = key, replicationCodes = value.ToList() });

            var resultList = new List<EntityInfo>();
            foreach (var entityType in list)
            {
                var entityIds = LookupEntities(_finder, entityType.EntityName, entityType.replicationCodes)
                    .Select(x => new EntityInfo { TypeName = entityType.EntityName, Id = x.Id })
                    .ToList();
                if (entityIds.Count != entityType.replicationCodes.Count)
                {
                    throw new ArgumentException("Some replication codes cannot be converted to entity identifiers", "replicationCodes");
                } 
               
                resultList.AddRange(entityIds);
            }

            return resultList;
        }

        public IEnumerable<Guid> ConvertToReplicationCodes(EntityName entityName, IEnumerable<long> entityIds)
        {
            throw new NotImplementedException();
        }

        private long LookupEntityId(EntityName entityName, Guid replicationCode)
        {
            var entity = LookupEntity(_finder, entityName, replicationCode);
            if (entity == null)
            {
                throw new ArgumentException(BLResources.CannotFindEntityByReplicationCode, "replicationCode");
            }

            var userCannotRead = LookupEntity(_secureFinder, entityName, replicationCode) == null;
            if (userCannotRead)
            {
                throw new ArgumentException(BLResources.CurrentUserHasNoReadEntityPermission, "replicationCode");
            }

            return entity.Id;
        }

        private static IReplicableEntity LookupEntity(IFinderBase finder, EntityName entityName, Guid replicationCode)
        {
            var findOneMethodInfo = finder.GetType().GetMethods().First(x => x.Name == "FindOne");
            if (findOneMethodInfo == null)
            {
                throw new ArgumentException("The finder does not have required method.", "finder");
            }

            var entityType = entityName.AsEntityType();

            var findSpecExpression = Expression.Call(
                ByReplicationCodeMethodInfo.MakeGenericMethod(entityType),
                Expression.Constant(replicationCode)
                );

            var findOneExpression = Expression.Call(
                Expression.Constant(finder),
                findOneMethodInfo.MakeGenericMethod(entityType),
                new Expression[] { findSpecExpression });

            var queryLambda = Expression.Lambda<Func<IReplicableEntity>>(findOneExpression).Compile();

            return queryLambda();
        }

        private static IEnumerable<IReplicableEntity> LookupEntities(IFinderBase finder, EntityName entityName, IEnumerable<Guid> replicationCodes)
        {
            var findManyMethodInfo = finder.GetType().GetMethods().First(x => x.Name == "FindMany");
            if (findManyMethodInfo == null)
            {
                throw new ArgumentException("The finder does not have required method.", "finder");
            }

            var entityType = entityName.AsEntityType();

            var findSpecExpression = Expression.Call(
                ByReplicationCodesMethodInfo.MakeGenericMethod(entityType),
                Expression.Constant(replicationCodes)
                );

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