using System;
using System.Linq;
using System.ServiceModel;
using System.Text;

using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using NuClear.Metamodeling.Domain.Operations.Detail;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Metadata
{
    public sealed class SoapApiOperationsMetadataProvider : SoapApiOperationServiceBase, IOperationsMetadataProvider
    {
        private const string AllOperationsCacheKey = "Alloperations";
        private const string CallerOperationsCacheKey = "CallerOperations";
        private const string ContextOperationsCacheKeyTemplate = "ContextOperations:{0}";

        private static readonly TimeSpan CachedAllOperationsExpiration = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan CachedCallerOperationsExpiration = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan CachedContextOperationsExpiration = TimeSpan.FromMinutes(1);

        private readonly ICacheAdapter _cacheAdapter;

        public SoapApiOperationsMetadataProvider(IStandartConfigurationSettings configuration,
                                                IDesktopClientProxyFactory clientProxyFactory,
                                                IApiSettings apiSettings,
                                                ITracer tracer,
                                                ICacheAdapter cacheAdapter)
            : base(clientProxyFactory, configuration, apiSettings, tracer)
        {
            _cacheAdapter = cacheAdapter;
        }

        public IOperationMetadata GetOperationMetadata(IOperationIdentity operationIdentity, params IEntityType[] operationProcessingEntities) 
        {
            IOperationMetadata metadataDetail;
            TryGetOperationMetadataDetail(operationIdentity, operationProcessingEntities, out metadataDetail);
            return metadataDetail;
        }

        public TOperationMetadata GetOperationMetadata<TOperationMetadata, TOperationIdentity>(params IEntityType[] operationProcessingEntities) 
            where TOperationMetadata : class, IOperationMetadata<TOperationIdentity> 
            where TOperationIdentity : IOperationIdentity, new()
        {
            IOperationMetadata metadataDetail;
            TryGetOperationMetadataDetail<TOperationIdentity>(operationProcessingEntities, out metadataDetail);
            return (TOperationMetadata)metadataDetail;
        }

        public bool IsSupported<TOperationIdentity>(params IEntityType[] operationProcessingEntities) 
            where TOperationIdentity : IOperationIdentity, new()
        {
            IOperationMetadata metadataDetail;
            return TryGetOperationMetadataDetail<TOperationIdentity>(operationProcessingEntities, out metadataDetail);
        }

        public OperationApplicability[] GetApplicableOperations()
        {
            OperationApplicability[] operations;

            if (_cacheAdapter.Contains(AllOperationsCacheKey))
            {
                operations = _cacheAdapter.Get<OperationApplicability[]>(AllOperationsCacheKey);
                if (operations != null)
                {
                    return operations;
                }
            }
            
            object faultDescription;
            var clientProxy = //ClientProxyFactory.GetClientProxy<IMetadataProviderApplicationService>("IMetadataProviderApplicationServiceSecure", Configuration.StandartConfiguration);
                              ClientProxyFactory.GetClientProxy<IMetadataProviderApplicationService, WSHttpBinding>();
            if (!clientProxy.TryExecuteWithFaultContract(x => x.GetApplicableOperations(), out operations, out faultDescription))
            {
                LogErrorAndThrow(faultDescription);
            }

            _cacheAdapter.Add(AllOperationsCacheKey, operations, CachedAllOperationsExpiration);

            return operations;
        }

        public OperationApplicability[] GetApplicableOperationsForCallingUser()
        {
            OperationApplicability[] operations;

            if (_cacheAdapter.Contains(CallerOperationsCacheKey))
            {
                operations = _cacheAdapter.Get<OperationApplicability[]>(CallerOperationsCacheKey);
                if (operations != null)
                {
                    return operations;
                }
            }

            object faultDescription;
            var clientProxy = ClientProxyFactory.GetClientProxy<IMetadataProviderApplicationService, WSHttpBinding>();
            if (!clientProxy.TryExecuteWithFaultContract(x => x.GetApplicableOperationsForCallingUser(), out operations, out faultDescription))
            {
                LogErrorAndThrow(faultDescription);
            }

            _cacheAdapter.Add(CallerOperationsCacheKey, operations, CachedCallerOperationsExpiration);

            return operations;
        }

        public OperationApplicability[] GetApplicableOperationsForContext(IEntityType[] entityNames, long[] entityIds)
        {
            if (entityNames == null || entityIds == null || entityNames.Length != entityIds.Length)
            {
                throw new ArgumentException("Specified arguments is invalid");
            }

            OperationApplicability[] operations;
            var sb = new StringBuilder();
            for (int i = 0; i < entityNames.Length; i++)
            {
                sb.AppendFormat("{0}={1}", entityNames[i].Id, entityIds[i]);
            }

            var cacheKey = string.Format(ContextOperationsCacheKeyTemplate, sb);
            if (_cacheAdapter.Contains(cacheKey))
            {
                operations = _cacheAdapter.Get<OperationApplicability[]>(cacheKey);
                if (operations != null)
                {
                    return operations;
                }
            }

            object faultDescription;
            var clientProxy = ClientProxyFactory.GetClientProxy<IMetadataProviderApplicationService, WSHttpBinding>();
            if (!clientProxy.TryExecuteWithFaultContract(x => x.GetApplicableOperationsForContext(entityNames, entityIds), out operations, out faultDescription))
            {
                LogErrorAndThrow(faultDescription);
            }

            _cacheAdapter.Add(cacheKey, operations, CachedContextOperationsExpiration);

            return operations;
        }

        private bool TryGetOperationMetadataDetail<TOperationIdentity>(IEntityType[] operationProcessingEntities, out IOperationMetadata metadataDetail)
            where TOperationIdentity : IOperationIdentity, new()
        {
            var targetIdentity = new TOperationIdentity();
            return TryGetOperationMetadataDetail(targetIdentity, operationProcessingEntities, out metadataDetail);
        }

        private bool TryGetOperationMetadataDetail(IOperationIdentity operationIdentity, IEntityType[] operationProcessingEntities, out IOperationMetadata metadataDetail)
        {
            metadataDetail = null;

            var availableOperations = GetApplicableOperations();
            var targetOperation = availableOperations.SingleOrDefault(o => o.OperationIdentity.Equals(operationIdentity));
            if (targetOperation == null)
            {
                return false;
            }

            var metadataDetailKey = operationProcessingEntities.ToEntitySet();
            return targetOperation.MetadataDetails.TryGetValue(metadataDetailKey, out metadataDetail);
        }

        private void LogErrorAndThrow(object faultDescription)
        {
            var desciption = (MetadataOperationErrorDescription)faultDescription;
            var errorDescription = "Api operation execution failed. " + desciption;
            Tracer.Error(errorDescription);
            throw new ApiException(errorDescription) { ApiExceptionDescription = new ApiExceptionDescriptor { Title = desciption.Message, Description = desciption.Description } };
        }
    }
}
