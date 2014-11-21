using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure
{
    public static class BindingConfiguration
    {
        public static WebHttpBindingBuilder WebHttp(string name)
        {
            return new WebHttpBindingBuilder(new WebHttpBinding
                {
                    MaxReceivedMessageSize = 10000000,
                    ReceiveTimeout = TimeSpan.FromHours(1),
                    SendTimeout = TimeSpan.FromHours(1),
                    Name = name
                });
        }

        public static WsHttpBindingBuilder WsHttp(string name)
        {
            return new WsHttpBindingBuilder(new WSHttpBinding
                {
                    MaxReceivedMessageSize = 10000000,
                    ReceiveTimeout = TimeSpan.FromHours(1),
                    SendTimeout = TimeSpan.FromHours(1),
                    Name = name
                });
        }

        public static BasicHttpBingindBuilder BasicHttp(string name)
        {
            return new BasicHttpBingindBuilder(new BasicHttpBinding
                {
                    MaxReceivedMessageSize = 10000000,
                    ReceiveTimeout = TimeSpan.FromHours(1),
                    SendTimeout = TimeSpan.FromHours(1),
                    Name = name
                });
        }
    }

    public class ServiceHostConfigurator
    {
        private readonly ServiceHost _serviceHost;

        public ServiceHostConfigurator(ServiceHost serviceHost)
        {
            _serviceHost = serviceHost;
        }

        public static implicit operator ServiceHost(ServiceHostConfigurator configurator)
        {
            return configurator._serviceHost;
        }

        public ServiceHostConfigurator AddEnpoint<TContract>(Uri uri, Binding binding)
        {
            _serviceHost.AddServiceEndpoint(typeof(TContract), binding, uri);

            return this;
        }
    }


    public class BaseBindingBuilder<TBuilder, TBinding>
        where TBinding : Binding
        where TBuilder : BaseBindingBuilder<TBuilder, TBinding>
    {
        protected readonly TBinding Binding;

        public BaseBindingBuilder(TBinding binding)
        {
            Binding = binding;
        }

        public static implicit operator TBinding(BaseBindingBuilder<TBuilder, TBinding> builder)
        {
            return builder.Binding;
        }

        public TBuilder Timeouts(TimeSpan recieveTimeout, TimeSpan sendTimeout)
        {
            Binding.ReceiveTimeout = recieveTimeout;
            Binding.SendTimeout = sendTimeout;
            return (TBuilder)this;
        }
    }

    public class BasicHttpBingindBuilder : BaseBindingBuilder<BasicHttpBingindBuilder, BasicHttpBinding>
    {
        public BasicHttpBingindBuilder(BasicHttpBinding binding) : base(binding)
        {
        }

        public BasicHttpBingindBuilder ClientCredentialType(HttpClientCredentialType type)
        {
            Binding.Security.Transport.ClientCredentialType = type;
            return this;
        }

        public BasicHttpBingindBuilder SecurityMode(BasicHttpSecurityMode mode)
        {
            Binding.Security.Mode = mode;
            return this;
        }

        public BasicHttpBingindBuilder MaxReceivedMessageSize(long messageSize)
        {
            Binding.MaxReceivedMessageSize = messageSize;
            return this;
        }
    }

    public class WebHttpBindingBuilder : BaseBindingBuilder<WebHttpBindingBuilder, WebHttpBinding>
    {
        public WebHttpBindingBuilder(WebHttpBinding binding)
            : base(binding)
        {
        }

        public WebHttpBindingBuilder EnableCrossDomainScriptAccess()
        {
            Binding.CrossDomainScriptAccessEnabled = true;
            return this;
        }

        public WebHttpBindingBuilder SecurityMode(WebHttpSecurityMode mode)
        {
            Binding.Security.Mode = mode;
            return this;
        }

        public WebHttpBindingBuilder ClientCredentialType(HttpClientCredentialType type)
        {
            Binding.Security.Transport.ClientCredentialType = type;
            return this;
        }

        public WebHttpBindingBuilder MaxReceivedMessageSize(long messageSize)
        {
            Binding.MaxReceivedMessageSize = messageSize;
            return this;
        }
    }

    public class WsHttpBindingBuilder : BaseBindingBuilder<WsHttpBindingBuilder, WSHttpBinding>
    {
        public WsHttpBindingBuilder(WSHttpBinding binding)
            : base(binding)
        {
        }

        public WsHttpBindingBuilder SecurityMode(SecurityMode mode)
        {
            Binding.Security.Mode = mode;
            return this;
        }

        public WsHttpBindingBuilder ClientCredentialType(HttpClientCredentialType type)
        {
            Binding.Security.Transport.ClientCredentialType = type;
            return this;
        }

        public WsHttpBindingBuilder MaxReceivedMessageSize(long messageSize)
        {
            Binding.MaxReceivedMessageSize = messageSize;
            return this;
        }
    }
}