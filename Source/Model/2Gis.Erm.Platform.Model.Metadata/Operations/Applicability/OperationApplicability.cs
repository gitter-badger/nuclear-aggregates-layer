using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail;

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
        private readonly EntityName[] _entities;
        [DataMember]
        private readonly IReadOnlyDictionary<EntityName, EntityName[]> _entitiesByAggregates;
        [DataMember]
        private readonly IReadOnlyDictionary<EntitySet, IOperationMetadata> _metadataDetails;
        
        public OperationApplicability(IOperationIdentity operationIdentity, IEnumerable<OperationMetadataDetailContainer> metadataDetails)
        {
            _operationIdentity = operationIdentity;
            _metadataDetails = metadataDetails.ToDictionary(c => c.SpecificTypes, c => c.MetadataDetail);

            var entitiesFlatList = new HashSet<EntityName>();
            var entitiesByAggregates = new Dictionary<EntityName, HashSet<EntityName>>();

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
                        HashSet<EntityName> entities;
                        if (!entitiesByAggregates.TryGetValue(aggregateRoot, out entities))
                        {
                            entities = new HashSet<EntityName>();
                            entitiesByAggregates.Add(aggregateRoot, entities);
                        }

                        entities.Add(entity);
                    }
                }   
            }

            _entitiesByAggregates = entitiesByAggregates.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
            _entities = entitiesFlatList.ToArray();
        }

        public EntityName[] Entities
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

        public IReadOnlyDictionary<EntityName, EntityName[]> EntitiesByAggregates
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
