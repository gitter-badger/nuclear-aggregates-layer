﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationResolver : IOperationResolver
    {
        private readonly IOperationIdentityRegistry _operationIdentityRegistry;
        private readonly ITracer _tracer;

        [Obsolete("Все новые PBO создаются с заполненным свойством OperationEntities, после того как необходимость обработки записей в старом формате исчезнет, либо сами записи исчезнут, нужно удалить всю логику вывода strictoperationidentities из PBO.descriptor")]
        private readonly IReadOnlyDictionary<int, EntitySet> _operationEntitiesMap;

        public OperationResolver(IOperationIdentityRegistry operationIdentityRegistry, ITracer tracer)
        {
            _operationIdentityRegistry = operationIdentityRegistry;
            _tracer = tracer;

            var operationEntitiesMap =
                EntityType.Instance.All()
                          .GetDecomposed()
                          .Where(x => !x.IsVirtual())
                          .Select(x => x.ToEntitySet())
                          .ToDictionary(entitySet => entitySet.Entities.EvaluateHash());

            var openEntitiesSet2Placeholders = new EntitySet(EntityType.Instance.All(), EntityType.Instance.All());
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
                var rawOperationEntityTypes = operation.OperationEntities.Split(';').ToArray();
                var entities = new IEntityType[rawOperationEntityTypes.Length];
                int processedIndex;
                for (processedIndex = 0; processedIndex < rawOperationEntityTypes.Length; processedIndex++)
                {
                    var rawOperationEntityType = rawOperationEntityTypes[processedIndex];
                    IEntityType entityName;
                    if (!EntityType.Instance.TryParse(int.Parse(rawOperationEntityType), out entityName))
                    {
                        _tracer.ErrorFormat("Can't parse value {0} from operation entities {1} as {2}",
                                            rawOperationEntityType,
                                            operation.OperationEntities,
                                            typeof(IEntityType).Name);
                        break;
                    }

                    entities[processedIndex] = entityName;
                }

                if (processedIndex != rawOperationEntityTypes.Length)
                {
                    _tracer.ErrorFormat("Can't parse some of the value with index {0} from operation entities {1} as {2}",
                                        processedIndex,
                                        operation.OperationEntities,
                                        typeof(IEntityType).Name);
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