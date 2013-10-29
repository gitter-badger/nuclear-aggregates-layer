using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;

namespace DoubleGis.Erm.Platform.WCF.Metadata
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public sealed class MetadataProviderApplicationService : IMetadataProviderApplicationService, IMetadataProviderApplicationRestService
    {
        private readonly IAppSettings _appSettings;
        private readonly IOperationsMetadataProvider _metadataProvider;
        private readonly IServiceAvailabilityProvider _serviceAvailabilityProvider;
        private readonly ICommonLog _logger;

        public MetadataProviderApplicationService(
            IAppSettings appSettings, 
            IOperationsMetadataProvider metadataProvider, 
            IServiceAvailabilityProvider serviceAvailabilityProvider,
            ICommonLog logger)
        {
            _appSettings = appSettings;
            _metadataProvider = metadataProvider;
            _serviceAvailabilityProvider = serviceAvailabilityProvider;
            _logger = logger;
        }

        public OperationApplicability[] GetApplicableOperations()
        {
            try
            {
                return _metadataProvider.GetApplicableOperations();
            }
            catch (Exception ex)
            {
                const string Message = "Can't get all applicable operations description";
                _logger.ErrorEx(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }

        public OperationApplicability[] GetApplicableOperationsForCallingUser()
        {
            try
            {
                return _metadataProvider.GetApplicableOperationsForCallingUser();
            }
            catch (Exception ex)
            {
                const string Message = "Can't get applicable operations for calling user description";
                _logger.ErrorEx(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }

        public OperationApplicability[] GetApplicableOperationsForContext(EntityName[] entityNames, long[] entityIds)
        {
            try
            {
                return _metadataProvider.GetApplicableOperationsForContext(entityNames, entityIds);
            }
            catch (Exception ex)
            {
                const string Message = "Can't get applicable operations for context description";
                _logger.ErrorEx(ex, Message);
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
                _logger.ErrorEx(ex, Message);
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
                _logger.ErrorEx(ex, Message);
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
                _logger.ErrorEx(ex, "Ошибка обработки запроса");
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
                _logger.ErrorEx(ex, "Ошибка обработки запроса");
                throw new WebFaultException(HttpStatusCode.InternalServerError);
            }
        }

        private FaultException<MetadataOperationErrorDescription> GetExceptionDescription(string operationSpecificMessage, Exception ex)
        {
            return new FaultException<MetadataOperationErrorDescription>(
                new MetadataOperationErrorDescription(operationSpecificMessage + ". " + ex.Message, _appSettings.TargetEnvironment != AppTargetEnvironment.Production ? ex.ToString() : string.Empty));
        }
    }
}
