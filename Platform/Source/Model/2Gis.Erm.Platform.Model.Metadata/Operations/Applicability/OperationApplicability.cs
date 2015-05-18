using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Aggregates;

using NuClear.Metamodeling.Domain.Operations.Detail;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability
{
    /// <summary>
    /// Описатель применимости операции (для каких сущностей реализована, какие обрабатывает и т.п.) 
    /// </summary>
    [DataContract]
    public sealed class OperationApplicability
    {
        [DataMember]
        private readonly IOperationIdentity _operationIdentity;
        [DataMember]
        private readonly IEntityType[] _entities;
        [DataMember]
        private readonly Dictionary<IEntityType, IEntityType[]> _entitiesByAggregates;
        [DataMember]
        private readonly Dictionary<EntitySet, IOperationMetadata> _metadataDetails;
        
        public OperationApplicability(IOperationIdentity operationIdentity, IEnumerable<OperationMetadataDetailContainer> metadataDetails)
        {
            _operationIdentity = operationIdentity;
            _metadataDetails = metadataDetails.ToDictionary(c => c.SpecificTypes, c => c.MetadataDetail);

            var entitiesFlatList = new HashSet<IEntityType>();
            var entitiesByAggregates = new Dictionary<IEntityType, HashSet<IEntityType>>();

            foreach (var operationEntitiesDescriptor in MetadataDetails.Keys)
            {
                foreach (var entity in operationEntitiesDescriptor.Entities)
                {
                    if (entitiesFlatList.Contains(entity))
                    {
                        continue;
                    }

                    entitiesFlatList.Add(entity);

                    var aggregates = entity.ToAggregates();
                    if (aggregates == null || aggregates.Length == 0)
                    {
                        continue;
                    }

                    foreach (var aggregateRoot in aggregates)
                    {
                        HashSet<IEntityType> entities;
                        if (!entitiesByAggregates.TryGetValue(aggregateRoot, out entities))
                        {
                            entities = new HashSet<IEntityType>();
                            entitiesByAggregates.Add(aggregateRoot, entities);
                        }

                        entities.Add(entity);
                    }
                }   
            }

            _entitiesByAggregates = entitiesByAggregates.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
            _entities = entitiesFlatList.ToArray();
        }

        public IEntityType[] Entities
        {
            get
            {
                return _entities;
            }
        }

        public IOperationIdentity OperationIdentity
        {
            get
            {
                return _operationIdentity;
            }
        }

        public IReadOnlyDictionary<IEntityType, IEntityType[]> EntitiesByAggregates
        {
            get
            {
                return _entitiesByAggregates;
            }
        }

        public IReadOnlyDictionary<EntitySet, IOperationMetadata> MetadataDetails
        {
            get
            {
                return _metadataDetails;
            }
        }
    }
}
