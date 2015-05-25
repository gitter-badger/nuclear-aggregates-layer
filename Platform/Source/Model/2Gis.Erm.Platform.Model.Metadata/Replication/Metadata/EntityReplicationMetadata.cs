using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class EntityReplicationMetadata
    {
        private EntityReplicationInfoBuilder _builder;

        private EntityReplicationMetadata()
        {
        }

        public static EntityReplicationInfoBuilder Config
        {
            get
            {
                var metadata = new EntityReplicationMetadata();
                var builder = EntityReplicationInfoBuilder.Create(metadata);
                metadata.Init(builder);

                return builder;
            }
        }

        public EntityReplicationInfoBuilder Then
        {
            get { return _builder; }
        }

        public IReadOnlyCollection<EntityReplicationInfo> Freeze()
        {
            return _builder.Build();
        }

        private void Init(EntityReplicationInfoBuilder builder)
        {
            _builder = builder;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal sealed class EntityReplicationInfoBuilder
    {
        private const string DefaultReplicateSpTemplate = "Replicate{0}";

        private readonly EntityReplicationMetadata _metadata;
        private readonly Dictionary<Type, Tuple<int, List<EntityReplicationInfo>>> _storage;
        private int _currentOrderValue;

        private EntityReplicationInfoBuilder(EntityReplicationMetadata metadata)
        {
            _metadata = metadata;
            _storage = new Dictionary<Type, Tuple<int, List<EntityReplicationInfo>>>();
            _currentOrderValue = -1;
        }

        public static EntityReplicationInfoBuilder Create(EntityReplicationMetadata metadata)
        {
            return new EntityReplicationInfoBuilder(metadata);
        }

        public EntityReplicationMetadata Single<TEntity>(string schemaName)
            where TEntity : class, IEntityKey
        {
            AddNextToStorage(typeof(TEntity), schemaName, string.Empty, ReplicationMode.Single);
            return _metadata;
        }

        public EntityReplicationMetadata Batch<TEntity>(string schemaName, string storedProcedureName)
            where TEntity : class, IEntityKey
        {
            AddNextToStorage(typeof(TEntity), schemaName, storedProcedureName, ReplicationMode.Batch);
            return _metadata;
        }

        public IReadOnlyCollection<EntityReplicationInfo> Build()
        {
            return _storage.Values.OrderBy(x => x.Item1).SelectMany(x => x.Item2).ToList();
        }

        private void AddNextToStorage(Type entityType, string schemaName, string storedProcedureName, ReplicationMode replicationMode)
        {
            var replicateSpName = storedProcedureName;
            if (string.IsNullOrEmpty(replicateSpName))
            {
                replicateSpName = string.Format(DefaultReplicateSpTemplate, entityType.Name);
            }

            Tuple<int, List<EntityReplicationInfo>> value;
            if (_storage.TryGetValue(entityType, out value))
            {
                value.Item2.Add(new EntityReplicationInfo(entityType, schemaName, replicateSpName, replicationMode));
            }
            else
            {
                value = Tuple.Create(++_currentOrderValue,
                                     new List<EntityReplicationInfo>
                                                 {
                                                     new EntityReplicationInfo(entityType, schemaName, replicateSpName, replicationMode)
                                                 });
                _storage.Add(entityType, value);
            }
        }
    }
}