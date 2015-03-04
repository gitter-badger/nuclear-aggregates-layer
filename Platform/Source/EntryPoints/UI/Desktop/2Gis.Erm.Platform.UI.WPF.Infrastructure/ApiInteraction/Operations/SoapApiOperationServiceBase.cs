using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations
{
    public abstract class SoapApiOperationServiceBase
    {
        private readonly IDesktopClientProxyFactory _clientProxyFactory;
        private readonly IStandartConfigurationSettings _configuration;
        private readonly IApiSettings _apiSettings;
        private readonly ICommonLog _logger;

        protected SoapApiOperationServiceBase(
            IDesktopClientProxyFactory clientProxyFactory,
            IStandartConfigurationSettings configuration,
            IApiSettings apiSettings,
            ICommonLog logger)
        {
            _clientProxyFactory = clientProxyFactory;
            _configuration = configuration;
            _apiSettings = apiSettings;
            _logger = logger;
        }

        protected ICommonLog Logger
        {
            get
            {
                return _logger;
            }
        }

        protected IDesktopClientProxyFactory ClientProxyFactory
        {
            get { return _clientProxyFactory; }
        }

        protected IStandartConfigurationSettings Configuration
        {
            get { return _configuration; }
        }

        protected IApiSettings ApiSettings
        {
            get { return _apiSettings; }
        }
    }
}