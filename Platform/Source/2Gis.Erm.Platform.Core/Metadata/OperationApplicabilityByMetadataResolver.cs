using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail;

using NuClear.Metamodeling.Domain.Operations.Detail;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Metadata
{
    public sealed class OperationApplicabilityByMetadataResolver : IOperationApplicabilityByMetadataResolver
    {
        /// <summary>
        /// Генерит карту доступности операций, используя:
        /// - данные соответствия операций и реализаций (entityspecific и noncoupled)
        /// - реестр metadata detail из сборки metadata
        /// </summary>
        public Dictionary<int, OperationApplicability> ResolveOperationsApplicability(
            IReadOnlyDictionary<Type, Dictionary<EntitySet, Type>> entitySpecificOperations,
            IReadOnlyDictionary<Type, Type> notCoupledOperations,
            IReadOnlyDictionary<Type, IOperationIdentity> operations2IdentitiesMap)
        {
            var operationApplicabilities = new Dictionary<int, OperationApplicability>();

            foreach (var entitySpecificOperation in entitySpecificOperations)
            {
                var metadataDetailMap = new Dictionary<EntitySet, OperationMetadataDetailContainer>();
                bool hasGenericImplementation = false;
                foreach (var implementation in entitySpecificOperation.Value)
                {
                    if (implementation.Key.IsOpenSet())
                    {
                        if (hasGenericImplementation)
                        {
                            throw new InvalidOperationException("More than one generic implementations detected for operation " + entitySpecificOperation.Key);
                        }

                        // generic реализацию раскладываем на составляющие - т.е. пытаемся отресолвить метаданные для всех возможных типов сущностей
                        foreach (var operationSpecificTypes in implementation.Key.ToConcreteSets())
                        {
                            if (metadataDetailMap.ContainsKey(operationSpecificTypes))
                            {   // для данного типа сущности уже были настроены metadataDetail - ничего не делаем, т.к. generic реализации имеют заведомо более низкий приоритет,
                                // чем специфические реализации
                                continue;
                            }

                            IOperationMetadata genericMetadataDetail;
                            if (!TryGetMetadataDetail(operations2IdentitiesMap, entitySpecificOperation.Key, operationSpecificTypes, out genericMetadataDetail))
                            {   // пока для generic реализаций считаем операцию недоступной для конкретного типа сущности, если для неё не удалось определить metadataDetail
                                continue;
                            }

                            metadataDetailMap.Add(
                                operationSpecificTypes,
                                new OperationMetadataDetailContainer
                                {
                                    MetadataDetail = genericMetadataDetail,
                                    SpecificTypes = operationSpecificTypes
                                });
                        }

                        hasGenericImplementation = true;
                        continue;
                    }

                    IOperationMetadata metadataDetail;
                    if (!TryGetMetadataDetail(operations2IdentitiesMap, entitySpecificOperation.Key, implementation.Key, out metadataDetail))
                    {   // не удалось определить метаданные
                        if (metadataDetailMap.ContainsKey(implementation.Key))
                        {   // метаданные специфичные для данного типа сущности(ей) уже были определены ранее не перетираем
                            continue;
                        }

                        var operationIdentity = GetOperationIdentity(operations2IdentitiesMap, entitySpecificOperation.Key);
                        var operationIdentityType = operationIdentity.GetType();
                        metadataDetail = EmptyOperationMetadata.Create.ForIdentityType(operationIdentityType);
                    }

                    metadataDetailMap[implementation.Key] =
                        new OperationMetadataDetailContainer
                        {
                            MetadataDetail = metadataDetail,
                            SpecificTypes = implementation.Key
                        };
                }

                var identity = GetOperationIdentity(operations2IdentitiesMap, entitySpecificOperation.Key);
                operationApplicabilities.Add(identity.Id, new OperationApplicability(identity, metadataDetailMap.Values.ToArray()));
            }

            foreach (var nonCoupledOperation in notCoupledOperations)
            {
                var identity = GetOperationIdentity(operations2IdentitiesMap, nonCoupledOperation.Key);
                operationApplicabilities.Add(identity.Id, new OperationApplicability(identity, new[] { OperationMetadataDetailContainer.Empty.NonCoupled }));
            }

            return operationApplicabilities;
        }

        private bool TryGetMetadataDetail(
            IReadOnlyDictionary<Type, IOperationIdentity> operations2IdentitiesMap, 
            Type operationType, 
            EntitySet operationSpecificTypes, 
            out IOperationMetadata metadataDetail)
        {
            var operationIdentityType = GetOperationIdentity(operations2IdentitiesMap, operationType);
            metadataDetail = OperationMetadataDetailRegistry.GetOperationMetadata(operationIdentityType.GetType(), operationSpecificTypes.Entities);
            return metadataDetail != null;
        }

        private IOperationIdentity GetOperationIdentity(IReadOnlyDictionary<Type, IOperationIdentity> operations2IdentitiesMap, Type operationType)
        {
            if (operationType == null)
            {
                throw new ArgumentNullException("operationType");
            }

            IOperationIdentity resolvedIdentity;
            if (!operations2IdentitiesMap.TryGetValue(operationType, out resolvedIdentity))
            {
                throw new InvalidOperationException("Can't resolve operation identity for specified operation type " + operationType);
            }

            return resolvedIdentity;
        }
    }
}
