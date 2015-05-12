using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Complete;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Reopen;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Simplified;

using Microsoft.Practices.Unity;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DI.Factories.Operations
{
    public sealed class UnityOperationServicesManager : IOperationServicesManager
    {
        private readonly IUnityContainer _container;
        private readonly IOperationsRegistry _operationsRegistry;
        private readonly IOperationsMetadataProvider _metadataProvider;

        public UnityOperationServicesManager(IUnityContainer container, IOperationsRegistry operationsRegistry, IOperationsMetadataProvider metadataProvider)
        {
            _container = container;
            _operationsRegistry = operationsRegistry;
            _metadataProvider = metadataProvider;
        }

        public IIntegrationProcessorOperationService GetOperationsExportService(IEntityType entityName, IEntityType integrationEntityName)
        {
            var entities = new[] { entityName, integrationEntityName };
            return GetEntitySpecificOperation<IIntegrationProcessorOperationService, ExportIdentity>(new EntitySet(entities), null);
        }

        public ICancelOperationService GetCancelService(IEntityType entityName)
        {            
            CheckOperationAvailability<CancelIdentity>(entityName);
            return GetEntitySpecificOperation<ICancelOperationService, CancelIdentity>(entityName.ToEntitySet(), null);
        }

        public ICompleteOperationService GetCompleteService(IEntityType entityName)
        {
            CheckOperationAvailability<CompleteIdentity>(entityName);
            return GetEntitySpecificOperation<ICompleteOperationService, CompleteIdentity>(entityName.ToEntitySet(), null);
        }

        public IReopenOperationService GetReopenService(IEntityType entityName)
        {
            CheckOperationAvailability<ReopenIdentity>(entityName);
            return GetEntitySpecificOperation<IReopenOperationService, ReopenIdentity>(entityName.ToEntitySet(), null);
        }

        public IListEntityService GetListEntityService(IEntityType entityName)
        {
            // пока не поддерживается CheckOperationAvailability<ListingMetadata, IListEntityService>(entityName);
            return GetEntitySpecificOperation<IListEntityService, ListIdentity>(entityName.ToEntitySet(), null);
        }
        
        public IDeleteEntityService GetDeleteEntityService(IEntityType entityName)
        {
            // пока не поддерживается CheckOperationAvailability<DeleteMetadata, IDeleteEntityService>(entityName);
            return GetEntitySpecificOperation<IDeleteEntityService, DeleteIdentity>(entityName.ToEntitySet(), typeof(IDeleteAggregateRepository<>));
        }

        public IAssignEntityService GetAssignEntityService(IEntityType entityName)
        {
            CheckOperationAvailability<AssignIdentity>(entityName);
            return GetEntitySpecificOperation<IAssignEntityService, AssignIdentity>(entityName.ToEntitySet(), typeof(IAssignAggregateRepository<>));
        }

        public IDeactivateEntityService GetDeactivateEntityService(IEntityType entityName)
        {
            // пока не поддерживается CheckOperationAvailability<DeactivateMetadata, IDeactivateEntityService>(entityName);
            return GetEntitySpecificOperation<IDeactivateEntityService, DeactivateIdentity>(entityName.ToEntitySet(), typeof(IDeactivateAggregateRepository<>));
        }

        public IActivateEntityService GetActivateEntityService(IEntityType entityName)
        {
            // пока не поддерживается CheckOperationAvailability<ActivateMetadata, IActivateEntityService>(entityName);
            return GetEntitySpecificOperation<IActivateEntityService, ActivateIdentity>(entityName.ToEntitySet(), typeof(IActivateAggregateRepository<>));
        }

        public IQualifyEntityService GetQualifyEntityService(IEntityType entityName)
        {
            CheckOperationAvailability<QualifyIdentity>(entityName);
            return GetEntitySpecificOperation<IQualifyEntityService, QualifyIdentity>(entityName.ToEntitySet(), null);
        }

        public IDisqualifyEntityService GetDisqualifyEntityService(IEntityType entityName)
        {
            CheckOperationAvailability<DisqualifyIdentity>(entityName);
            return GetEntitySpecificOperation<IDisqualifyEntityService, DisqualifyIdentity>(entityName.ToEntitySet(), null);
        }

        public ICheckEntityForDebtsService GetCheckEntityForDebtsService(IEntityType entityName)
        {
            CheckOperationAvailability<CheckForDebtsIdentity>(entityName);
            return GetEntitySpecificOperation<ICheckEntityForDebtsService, CheckForDebtsIdentity>(entityName.ToEntitySet(), null);
        }

        public IChangeEntityTerritoryService GetChangeEntityTerritoryService(IEntityType entityName)
        {
            CheckOperationAvailability<ChangeTerritoryIdentity>(entityName);
            return GetEntitySpecificOperation<IChangeEntityTerritoryService, ChangeTerritoryIdentity>(entityName.ToEntitySet(), null);
        }

        public IChangeEntityClientService GetChangeEntityClientService(IEntityType entityName)
        {
            CheckOperationAvailability<ChangeClientIdentity>(entityName);
            return GetEntitySpecificOperation<IChangeEntityClientService, ChangeClientIdentity>(entityName.ToEntitySet(), null);
        }

        public IAppendEntityService GetAppendEntityService(IEntityType parentType, IEntityType appendedType)
        {
            var entities = new[] { appendedType, parentType };
            CheckOperationAvailability<AppendIdentity>(entities);
            return GetEntitySpecificOperation<IAppendEntityService, AppendIdentity>(new EntitySet(entities), null);
        }

        public IActionsHistoryService GetActionHistoryService(IEntityType entityName)
        {
            CheckOperationAvailability<ActionHistoryIdentity>(entityName);
            return GetNotCoupledOperation<IActionsHistoryService, ActionHistoryIdentity>();
        }

        public IGetDomainEntityDtoService GetDomainEntityDtoService(IEntityType entityName)
        {
            return GetEntitySpecificOperation<IGetDomainEntityDtoService, GetDomainEntityDtoIdentity>(entityName.ToEntitySet(), null);
        }

        public IModifyDomainEntityService GetModifyDomainEntityService(IEntityType entityName)
        {
            IModifyDomainEntityService modifyDomainEntityService;
            if (SimplifiedEntities.Entities.Any(x => x == entityName))
            {
                CheckOperationAvailability<ModifySimplifiedModelEntityIdentity>(entityName);
                modifyDomainEntityService = GetEntitySpecificOperation<IModifySimplifiedModelEntityService, ModifySimplifiedModelEntityIdentity>(entityName.ToEntitySet(), null)
                                            as IModifyDomainEntityService;
            }
            else if (AggregatesList.Aggregates.Any(x => x.Value.AggregateEntities.Any(y => y == entityName)))
            {
                CheckOperationAvailability<ModifyBusinessModelEntityIdentity>(entityName);
                modifyDomainEntityService = GetEntitySpecificOperation<IModifyBusinessModelEntityService, ModifyBusinessModelEntityIdentity>(entityName.ToEntitySet(), null)
                                            as IModifyDomainEntityService;
            }
            else
            {
                throw new NotSupportedException("Domain entity must be either from business model or simplified model");
            }

            if (modifyDomainEntityService == null)
            {
                throw new NotSupportedException("Modify domain entity service must implement IModifyDomainEntityService interface");
            }

            return modifyDomainEntityService;
        }

        public IDownloadFileService GetDownloadFileService(IEntityType entityName)
        {
            CheckOperationAvailability<DownloadIdentity>(entityName);
            return GetEntitySpecificOperation<IDownloadFileService, DownloadIdentity>(entityName.ToEntitySet(), null);
        }

        public IUploadFileService GetUploadFileService(IEntityType entityName)
        {
            CheckOperationAvailability<UploadIdentity>(entityName);
            return GetEntitySpecificOperation<IUploadFileService, UploadIdentity>(entityName.ToEntitySet(), null);
        }

        private TOperation GetEntitySpecificOperation<TOperation, TOperationIdentity>(EntitySet descriptor, Type overrideGenericAggregateRepository)
            where TOperation : class, IOperation<TOperationIdentity>
            where TOperationIdentity : class, IOperationIdentity, new()
        {
            EntitySet resolvedDescriptor;
            Type resolvedImplementationType;
            if (!_operationsRegistry.TryGetEntitySpecificOperation<TOperation>(descriptor, out resolvedImplementationType, out resolvedDescriptor))
            {
                throw new NotSupportedException("Can't find specified entity specific operation in supported operations registry. Operation type: " + typeof(TOperation));
            }

            if (!resolvedDescriptor.IsOpenSet())
            {
                return _container.Resolve<TOperation>(resolvedDescriptor.ToString()); // _container.Resolve(resolvedImplementationType);
            }

            // нужно использовать generic реализацию операции
            if (resolvedDescriptor.Entities == null || resolvedDescriptor.Entities.Length == 0)
            {
                throw new NotSupportedException("Can't process generic aggregate repository for not specified entity type" + descriptor);
            }

            if (resolvedDescriptor.Entities.Length > 1)
            {
                throw new NotSupportedException("Can't process generic aggregate repository dependant on several entities types. " + resolvedDescriptor);
            }

            var entityType = descriptor.Entities[0].AsEntityType();
            var targetImplementationType = resolvedImplementationType.MakeGenericType(entityType);
            if (overrideGenericAggregateRepository == null)
            {   // override для generic агрегирующего репозитория не нужен, скорее всего реализация сервиса просто не зависит от агрегирующего репозитория,
                // например, это stub реализация бросающая исключения при вызове
                return (TOperation)_container.Resolve(targetImplementationType);
            }

            if (!typeof(IAggregateRepository).IsAssignableFrom(overrideGenericAggregateRepository))
            {
                throw new InvalidOperationException("Can't specified type can't be uesed as aggregate repository. Type: " + overrideGenericAggregateRepository);
            }

            var mapping = entityType.IsSecurableAccessRequired() ? Mapping.SecureOperationRepositoriesScope : Mapping.UnsecureOperationRepositoriesScope;
            var targetAggregateRepositoryType = overrideGenericAggregateRepository.MakeGenericType(entityType);
            return (TOperation)_container.Resolve(targetImplementationType, OverrideDependencyScope(targetAggregateRepositoryType, mapping));
        }

        private TOperation GetNotCoupledOperation<TOperation, TOperationIdentity>()
            where TOperation : class, IOperation<TOperationIdentity>
            where TOperationIdentity : class, IOperationIdentity, new()
        {
            Type resolvedImplementationType;
            if (!_operationsRegistry.TryGetNotCoupledOperation<TOperation>(out resolvedImplementationType))
            {
                throw new NotSupportedException("Can't find specified notcoupled operation in supported operations registry. Operation type: " + typeof(TOperation));
            }

            return (TOperation)_container.Resolve(resolvedImplementationType);
        }

        private void CheckOperationAvailability<TOperationIdentity>(params IEntityType[] operationProcessingEntities)
            where TOperationIdentity : IOperationIdentity, new()
        {
            if (!_metadataProvider.IsSupported<TOperationIdentity>(operationProcessingEntities))
            {
                throw new NotSupportedException(
                    string.Format("Operation of type: {0} is not supported for entities types: {1}", typeof(TOperationIdentity), operationProcessingEntities.EntitiesToString()));
            }
        }

        private static DependencyOverride OverrideDependencyScope(Type type, string scope)
        {
            return new DependencyOverride(type, new ResolvedParameter(type, scope));
        }
    }
}