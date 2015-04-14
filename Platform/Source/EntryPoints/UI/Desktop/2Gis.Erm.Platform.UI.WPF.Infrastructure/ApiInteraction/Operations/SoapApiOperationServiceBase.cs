using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations
{
    public abstract class SoapApiOperationServiceBase
    {
        private readonly IDesktopClientProxyFactory _clientProxyFactory;
        private readonly IStandartConfigurationSettings _configuration;
        private readonly IApiSettings _apiSettings;
        private readonly ITracer _tracer;

        protected SoapApiOperationServiceBase(
            IDesktopClientProxyFactory clientProxyFactory,
            IStandartConfigurationSettings configuration,
            IApiSettings apiSettings,
            ITracer tracer)
        {
            _clientProxyFactory = clientProxyFactory;
            _configuration = configuration;
            _apiSettings = apiSettings;
            _tracer = tracer;
        }

        protected ITracer Tracer
        {
            get
            {
                return _tracer;
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