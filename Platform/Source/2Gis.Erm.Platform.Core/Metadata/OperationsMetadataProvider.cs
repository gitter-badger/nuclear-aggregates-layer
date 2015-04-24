using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail;

namespace DoubleGis.Erm.Platform.Core.Metadata
{
    public sealed class OperationsMetadataProvider : IOperationsMetadataProvider
    {
        private readonly IOperationAcceptabilityRegistrar _operationAcceptabilityRegistrar;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceEntityAccess _serviceEntityAccess;
        private readonly ISecurityServiceFunctionalAccess _serviceFunctionalAccess;
        private readonly IOperationSecurityRegistryReader _operationSecurityRegistryReader;

        public OperationsMetadataProvider(
            IOperationAcceptabilityRegistrar operationAcceptabilityRegistrar,
            IUserContext userContext,
            ISecurityServiceEntityAccess serviceEntityAccess,
            ISecurityServiceFunctionalAccess serviceFunctionalAccess,
            IOperationSecurityRegistryReader operationSecurityRegistryReader)
        {
            _operationAcceptabilityRegistrar = operationAcceptabilityRegistrar;
            _userContext = userContext;
            _serviceEntityAccess = serviceEntityAccess;
            _serviceFunctionalAccess = serviceFunctionalAccess;
            _operationSecurityRegistryReader = operationSecurityRegistryReader;
        }

        #region Implementation of IOperationMetadataProvider

        public IOperationMetadata GetOperationMetadata(IOperationIdentity operationIdentity, params EntityName[] operationProcessingEntities)
        {
            return OperationMetadataDetailRegistry.GetOperationMetadata(operationIdentity.GetType(), operationProcessingEntities);
        }

        public TOperationMetadata GetOperationMetadata<TOperationMetadata, TOperationIdentity>(params EntityName[] operationProcessingEntities)
            where TOperationMetadata : class, IOperationMetadata<TOperationIdentity>
            where TOperationIdentity : IOperationIdentity, new()
        {
            return (TOperationMetadata)OperationMetadataDetailRegistry.GetOperationMetadata<TOperationIdentity>(operationProcessingEntities);
        }

        public bool IsSupported<TOperationIdentity>(params EntityName[] operationProcessingEntities) where TOperationIdentity : IOperationIdentity, new()
        {
            // пока простейшая реализация, без кэширования и т.п.
            return OperationMetadataDetailRegistry.GetOperationMetadata<TOperationIdentity>(operationProcessingEntities) != null;
        }

        public OperationApplicability[] GetApplicableOperations()
        {
            return _operationAcceptabilityRegistrar.InitialOperationApplicability.Values.ToArray();
        }

        public OperationApplicability[] GetApplicableOperationsForCallingUser()
        {
            return RestrictAccessability(new Dictionary<EntityName, long>());
        }

        public OperationApplicability[] GetApplicableOperationsForContext(EntityName[] entityNames, long[] entityIds)
        {
            return RestrictAccessability(GetTargetEntitiesInstanceMap(entityNames, entityIds));
        }

        #endregion

        private IReadOnlyDictionary<EntityName, long> GetTargetEntitiesInstanceMap(EntityName[] entityNames, long[] entityIds)
        {
            var specificInstances = new Dictionary<EntityName, long>();

            if (entityNames != null && entityIds != null)
            {
                if (entityNames.Length != entityIds.Length)
                {
                    throw new ArgumentException("Specified entities types and ids count are mismatch");
                }

                for (int i = 0; i < entityNames.Length; i++)
                {
                    specificInstances.Add(entityNames[i], entityIds[i]);
                }
            }

            return specificInstances;
        }

        private OperationApplicability[] RestrictAccessability(IReadOnlyDictionary<EntityName, long> instanceMap)
        {
            var restrictedOperations = new List<OperationApplicability>();

            foreach (var operation in _operationAcceptabilityRegistrar.InitialOperationApplicability)
            {
                var initialApplicability = operation.Value;
                var restrictedMetadataDetails = new List<OperationMetadataDetailContainer>();

                IEnumerable<IAccessRequirement> securityRequirements;

                foreach (var metadataDetail in initialApplicability.MetadataDetails)
                {
                    var operationIdentity = new StrictOperationIdentity(initialApplicability.OperationIdentity, metadataDetail.Key);
                    if (!_operationSecurityRegistryReader.TryGetOperationRequirements(operationIdentity, out securityRequirements))
                    {   
                        // для операции не указано специфических требований безопасности - считаем, что она разрешена
                        restrictedOperations.Add(initialApplicability);
                        continue;
                    }

                    if (!IsAccessable(metadataDetail.Key, securityRequirements, instanceMap))
                    {
                        continue;
                    }

                    restrictedMetadataDetails.Add(new OperationMetadataDetailContainer
                        {
                            SpecificTypes = metadataDetail.Key,
                            MetadataDetail = metadataDetail.Value
                        });
                }

                if (restrictedMetadataDetails.Count > 0)
                {
                    restrictedOperations.Add(new OperationApplicability(initialApplicability.OperationIdentity, restrictedMetadataDetails));
                }
            }

            return restrictedOperations.ToArray();
        }

        private bool IsAccessable(
            EntitySet checkingOperationSpecificTypes,
            IEnumerable<IAccessRequirement> securityRequirement,
            IReadOnlyDictionary<EntityName, long> instanceMap)
        {
            var notDefinedEntitiesIndicator = EntityName.None.ToEntitySet();
            var userCode = _userContext.Identity.Code;

            if (!checkingOperationSpecificTypes.Equals(notDefinedEntitiesIndicator))
            {
                foreach (var checkingEntityName in checkingOperationSpecificTypes.Entities)
                {
                    var accessTypes = securityRequirement.OfType<EntityAccessRequirement>()
                                                         .Where(requirement => requirement.EntityName == checkingEntityName)
                                                         .Aggregate((EntityAccessTypes)0, (accumulator, value) => accumulator | value.EntityAccessType);

                    long? targetEntityId = null;
                    long entityId;
                    if (instanceMap.TryGetValue(checkingEntityName, out entityId))
                    {
                        targetEntityId = entityId;
                    }

                    // TODO пока вместо реальных значений используются stub - Assign и т.п. могут работать не правильно
                    const long Owner = 0;
                    long? oldOwner = null;

                    if (!_serviceEntityAccess.HasEntityAccess(accessTypes,
                                                              checkingEntityName,
                                                              userCode,
                                                              targetEntityId,
                                                              Owner,
                                                              oldOwner))
                    {
                        return false;
                    }
                }
            }

            foreach (var functionalPrivilege in securityRequirement.OfType<FunctionalAccessRequirement>())
            {
                if (!_serviceFunctionalAccess.HasFunctionalPrivilegeGranted(functionalPrivilege.AccessType, userCode))
                {
                    return false;
                }
            }

            return true;
        }
    }
}