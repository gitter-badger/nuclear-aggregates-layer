using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationResolver : IOperationResolver
    {
        private readonly IOperationIdentityRegistry _operationIdentityRegistry;
        private readonly ICommonLog _logger;

        [Obsolete("Все новые PBO создаются с заполненным свойством OperationEntities, после того как необходимость обработки записей в старом формате исчезнет, либо сами записи исчезнут, нужно удалить всю логику вывода strictoperationidentities из PBO.descriptor")]
        private readonly IReadOnlyDictionary<int, EntitySet> _operationEntitiesMap;

        public OperationResolver(IOperationIdentityRegistry operationIdentityRegistry, ICommonLog logger)
        {
            _operationIdentityRegistry = operationIdentityRegistry;
            _logger = logger;

            var operationEntitiesMap =
                EntityName.All
                          .GetDecomposed()
                          .Where(x => !x.IsVirtual())
                          .Select(x => x.ToEntitySet())
                          .ToDictionary(entitySet => entitySet.Entities.EvaluateHash());

            var openEntitiesSet2Placeholders = new EntitySet(EntityName.All, EntityName.All);
            foreach (var entitySet in openEntitiesSet2Placeholders.ToConcreteSets())
            {
                operationEntitiesMap.Add(entitySet.Entities.EvaluateHash(), entitySet);
            }

            _operationEntitiesMap = operationEntitiesMap;
        }

        public StrictOperationIdentity ResolveOperation(PerformedBusinessOperation operation)
        {
            var operationIdentity = _operationIdentityRegistry.GetIdentity(operation.Operation);
            EntitySet operationEntities = null;
            if (!string.IsNullOrWhiteSpace(operation.OperationEntities))
            {
                var rawOperationEntities = operation.OperationEntities.Split(';').ToArray();
                var entities = new EntityName[rawOperationEntities.Length];
                int processedIndex;
                for (processedIndex = 0; processedIndex < rawOperationEntities.Length; processedIndex++)
                {
                    var rawOperationEntity = rawOperationEntities[processedIndex];
                    EntityName entityName;
                    if (!Enum.TryParse(rawOperationEntity, out entityName))
                    {
                        _logger.ErrorFormat("Can't parse value {0} from operation entities {1} as {2}",
                                              rawOperationEntity,
                                              operation.OperationEntities,
                                              typeof(EntityName).Name);
                        break;
                    }

                    entities[processedIndex] = entityName;
                }

                if (processedIndex != rawOperationEntities.Length)
                {
                    _logger.ErrorFormat("Can't parse some of the value with index {0} from operation entities {1} as {2}",
                                          processedIndex,
                                          operation.OperationEntities,
                                          typeof(EntityName).Name);
                }
                else
                {
                    operationEntities = new EntitySet(entities);
                }
            }

            if (operationEntities == null)
            {
                operationEntities = operation.Descriptor == 0 ? EntitySet.Create.NonCoupled : _operationEntitiesMap[operation.Descriptor];
            }

            return new StrictOperationIdentity(operationIdentity, operationEntities);
        }
    }
}