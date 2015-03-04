using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.WCF.Metadata
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public sealed class MetadataProviderApplicationService : IMetadataProviderApplicationService, IMetadataProviderApplicationRestService
    {
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly IOperationsMetadataProvider _metadataProvider;
        private readonly IServiceAvailabilityProvider _serviceAvailabilityProvider;
        private readonly ICommonLog _logger;

        public MetadataProviderApplicationService(
            IEnvironmentSettings environmentSettings, 
            IOperationsMetadataProvider metadataProvider, 
            IServiceAvailabilityProvider serviceAvailabilityProvider,
            ICommonLog logger)
        {
            _environmentSettings = environmentSettings;
            _metadataProvider = metadataProvider;
            _serviceAvailabilityProvider = serviceAvailabilityProvider;
            _logger = logger;
        }

        public OperationApplicability[] GetApplicableOperations()
        {
            try
            {
                return _metadataProvider
                            .GetApplicableOperations()
                            .Where(NotRestrictedForClients)
                            .ToArray();
            }
            catch (Exception ex)
            {
                const string Message = "Can't get all applicable operations description";
                _logger.Error(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }

        public OperationApplicability[] GetApplicableOperationsForCallingUser()
        {
            try
            {
                return _metadataProvider
                            .GetApplicableOperationsForCallingUser()
                            .Where(NotRestrictedForClients)
                            .ToArray();
            }
            catch (Exception ex)
            {
                const string Message = "Can't get applicable operations for calling user description";
                _logger.Error(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }

        public OperationApplicability[] GetApplicableOperationsForContext(EntityName[] entityNames, long[] entityIds)
        {
            try
            {
                return _metadataProvider
                                .GetApplicableOperationsForContext(entityNames, entityIds)
                                .Where(NotRestrictedForClients)
                                .ToArray();
            }
            catch (Exception ex)
            {
                const string Message = "Can't get applicable operations for context description";
                _logger.Error(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }

        public EndpointDescription[] GetCompatibleServices(Version clientVersion)
        {
            try
            {
                return _serviceAvailabilityProvider.GetCompatibleServices(clientVersion);
            }
            catch (Exception ex)
            {
                const string Message = "Can't get applicable operations for context description";
                _logger.Error(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }

        public EndpointDescription[] GetAvailableServices()
        {
            try
            {
                return _serviceAvailabilityProvider.GetAvailableServices();
            }
            catch (Exception ex)
            {
                const string Message = "Can't get applicable operations for context description";
                _logger.Error(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }

        EndpointDescription[] IMetadataProviderApplicationRestService.GetAvailableServices()
        {
            try
            {
                return GetAvailableServices();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка обработки запроса");
                throw new WebFaultException(HttpStatusCode.InternalServerError);
            }
        }

        EndpointDescription[] IMetadataProviderApplicationRestService.GetCompatibleServices(string clientVersion)
        {
            try
            {
                return GetCompatibleServices(Version.Parse(clientVersion));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка обработки запроса");
                throw new WebFaultException(HttpStatusCode.InternalServerError);
            }
        }

        private bool NotRestrictedForClients(OperationApplicability operationApplicability)
        {
            // TODO {all, 24.04.2014}: пока примитивное ограничение для перечня операций - исключаем те которые непредназначены для remote клиента (например, потому что не помечены DataContractAttribute)
            return operationApplicability.OperationIdentity.GetType().GetCustomAttributes<DataContractAttribute>().Any()
                && operationApplicability.MetadataDetails.All(pair => pair.Value != null && pair.Value.GetType().GetCustomAttributes<DataContractAttribute>().Any());
        }

        private FaultException<MetadataOperationErrorDescription> GetExceptionDescription(string operationSpecificMessage, Exception ex)
        {
            return new FaultException<MetadataOperationErrorDescription>(
                new MetadataOperationErrorDescription(operationSpecificMessage + ". " + ex.Message, _environmentSettings.Type != EnvironmentType.Production ? ex.ToString() : string.Empty));
        }
    }
}
