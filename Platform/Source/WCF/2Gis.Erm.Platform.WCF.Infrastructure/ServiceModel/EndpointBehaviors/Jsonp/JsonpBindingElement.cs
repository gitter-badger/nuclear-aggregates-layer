using System;
using System.ServiceModel.Channels;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.Jsonp
{
    public class JsonpBindingElement : MessageEncodingBindingElement
    {
        private readonly WebMessageEncodingBindingElement _webMessageEncodingBindingElement = new WebMessageEncodingBindingElement();

        public override MessageVersion MessageVersion
        {
            get { return _webMessageEncodingBindingElement.MessageVersion; }
            set { throw new InvalidOperationException("MessageVersion property cannot be set."); }
        }

        public override BindingElement Clone()
        {
            return new JsonpBindingElement();
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new JsonpEncoderFactory();
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Context is null.");
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Context is null.");
            }

            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Context is null.");
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Context is null.");
            }

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return _webMessageEncodingBindingElement.GetProperty<T>(context);
        }
    }
}