using System;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.WCF.Metadata
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public sealed class IdentityProviderApplicationService : IIdentityProviderApplicationService, IIdentityProviderApplicationRestService
    {
        private readonly IAppSettings _appSettings;
        private readonly IIdentityProviderService _identityService;
        private readonly ICommonLog _logger;

        public IdentityProviderApplicationService(IAppSettings appSettings,
                                                  IIdentityProviderService identityService,
                                                  ICommonLog logger)
        {
            _appSettings = appSettings;
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
                _logger.ErrorEx(ex, Message);
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
                _logger.ErrorEx(ex, Message);
                throw GetExceptionDescription(Message, ex);
            }
        }
        
        private FaultException<MetadataOperationErrorDescription> GetExceptionDescription(string operationSpecificMessage, Exception ex)
        {
            return new FaultException<MetadataOperationErrorDescription>(
                new MetadataOperationErrorDescription(operationSpecificMessage + ". " + ex.Message, _appSettings.TargetEnvironment != AppTargetEnvironment.Production ? ex.ToString() : string.Empty));
        }
    }
}
