using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Config
{
    public static class BindingConfig
    {
        public static WebHttpBindingBuilder WebHttp
        {
            get
            {
                return new WebHttpBindingBuilder(new WebHttpBinding
                    {
                        MaxReceivedMessageSize = 10000000,
                        ReceiveTimeout = TimeSpan.FromHours(1),
                        SendTimeout = TimeSpan.FromHours(1)
                    });
            }
        }

        public static WsHttpBindingBuilder WsHttp
        {
            get
            {
                return new WsHttpBindingBuilder(new WSHttpBinding
                    {
                        MaxReceivedMessageSize = 10000000,
                        ReceiveTimeout = TimeSpan.FromHours(1),
                        SendTimeout = TimeSpan.FromHours(1)
                    });
            }
        }

        public static BasicHttpBindingBuilder BasicHttp
        {
            get
            {
                return new BasicHttpBindingBuilder(new BasicHttpBinding
                    {
                        MaxReceivedMessageSize = 10000000,
                        ReceiveTimeout = TimeSpan.FromHours(1),
                        SendTimeout = TimeSpan.FromHours(1)
                    });
            }
        }

        public static WsDualHttpBindingBuilder WsDualHttp
        {
            get
            {
                return new WsDualHttpBindingBuilder(new WSDualHttpBinding()
                {
                    MaxReceivedMessageSize = 10000000,
                    ReceiveTimeout = TimeSpan.FromHours(1),
                    SendTimeout = TimeSpan.FromHours(1)
                });
            }
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

        public TBuilder Name(string name)
        {
            Binding.Name = name;
            return (TBuilder)this;
        }
    }

    public class BasicHttpBindingBuilder : BaseBindingBuilder<BasicHttpBindingBuilder, BasicHttpBinding>
    {
        public BasicHttpBindingBuilder(BasicHttpBinding binding) : base(binding)
        {
        }

        public BasicHttpBindingBuilder UseTransportSecurity(HttpClientCredentialType type)
        {
            Binding.Security.Mode = BasicHttpSecurityMode.Transport;
            Binding.Security.Transport.ClientCredentialType = type;
            return this;
        }

        public BasicHttpBindingBuilder UseMessageSecurity(BasicHttpMessageCredentialType type)
        {
            Binding.Security.Mode = BasicHttpSecurityMode.Message;
            Binding.Security.Message.ClientCredentialType = type;
            return this;
        }

        public BasicHttpBindingBuilder MaxReceivedMessageSize(long messageSize)
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

        public WebHttpBindingBuilder UseTransportSecurity(HttpClientCredentialType type)
        {
            Binding.Security.Mode = WebHttpSecurityMode.Transport;
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

        public WsHttpBindingBuilder UseMessageSecurity(MessageCredentialType type)
        {
            Binding.Security.Mode = System.ServiceModel.SecurityMode.Message;
            Binding.Security.Message.ClientCredentialType = type;
            return this;
        }

        public WsHttpBindingBuilder UseTransportSecurity(HttpClientCredentialType type)
        {
            Binding.Security.Mode = System.ServiceModel.SecurityMode.Transport;
            Binding.Security.Transport.ClientCredentialType = type;
            return this;
        }

        public WsHttpBindingBuilder MaxReceivedMessageSize(long messageSize)
        {
            Binding.MaxReceivedMessageSize = messageSize;
            return this;
        }
    }

    public class WsDualHttpBindingBuilder : BaseBindingBuilder<WsDualHttpBindingBuilder, WSDualHttpBinding>
    {
        public WsDualHttpBindingBuilder(WSDualHttpBinding binding) : base(binding)
        {
        }

        public WsDualHttpBindingBuilder ClientBaseAddress(string clientBaseAddress)
        {
            Binding.ClientBaseAddress = new Uri(clientBaseAddress);
            return this;
        }
    }
}