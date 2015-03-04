using System;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.Common.Identities;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.WCF.Metadata
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public sealed class IdentityProviderApplicationService : IIdentityProviderApplicationService, IIdentityProviderApplicationRestService
    {
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly IIdentityProviderService _identityService;
        private readonly ITracer _logger;

        public IdentityProviderApplicationService(IEnvironmentSettings environmentSettings,
                                                  IIdentityProviderService identityService,
                                                  ITracer logger)
        {
            _environmentSettings = environmentSettings;
            _identityService = identityService;
            _logger = logger;
        }

        public long[] GetIdentities(int count)
        {
            try
            {
                return _identityService.GetIdentities(count);
            }
            catch (Exception ex)
            {
                const string Message = "Can't generate requested identities";
                _logger.Error(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }

        public long NewIdentity()
        {
            try
            {
                return _identityService.GetIdentities(1).Single();
            }
            catch (Exception ex)
            {
                const string Message = "Can't generate requested identities";
                _logger.Error(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }
        
        private FaultException<MetadataOperationErrorDescription> GetExceptionDescription(string operationSpecificMessage, Exception ex)
        {
            return new FaultException<MetadataOperationErrorDescription>(
                new MetadataOperationErrorDescription(operationSpecificMessage + ". " + ex.Message, _environmentSettings.Type != EnvironmentType.Production ? ex.ToString() : string.Empty));
        }
    }
}
