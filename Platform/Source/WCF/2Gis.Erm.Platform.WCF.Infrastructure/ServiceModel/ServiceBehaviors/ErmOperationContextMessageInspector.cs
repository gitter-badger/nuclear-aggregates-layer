using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors.Lifetime;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors
{
    /// <summary>
    /// Adds and removes instances of <see cref="LifetimeManagementOperationContextExtension"/> from the current operation context.
    /// </summary>
    public class ErmOperationContextMessageInspector : IDispatchMessageInspector
    {
        private readonly ILoggerContextManager _loggerContextManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErmOperationContextMessageInspector"/> class.
        /// </summary>
        /// <param name="loggerContextManager"></param>
        public ErmOperationContextMessageInspector(ILoggerContextManager loggerContextManager)
        {
            _loggerContextManager = loggerContextManager;
        }

        /// <summary>
        /// Adds an extensions to the current operation context after an inbound message has been received but before the message is dispatched to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>The object used to correlate state. This object is passed back in the BeforeSendReply method.</returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var currentOperationContext = OperationContext.Current;
            
            currentOperationContext.Extensions.Add(new LifetimeManagementOperationContextExtension());
            currentOperationContext.Extensions.Add(new Log4NetConfigurationOperationContextExtension());
            
            SetOperationContextProperties(currentOperationContext);

            return null;
        }

        /// <summary>
        /// Removes the registered instance of the extensions from the current operation context after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">The reply message. This value is null if the operation is one way.</param>
        /// <param name="correlationState">The correlation object returned from the AfterReceiveRequest method.</param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            OperationContext.Current.Extensions.Remove(LifetimeManagementOperationContextExtension.Current);
            OperationContext.Current.Extensions.Remove(Log4NetConfigurationOperationContextExtension.Current);
        }

        private void SetOperationContextProperties(OperationContext currentOperationContext)
        {
            var securityServiceContext = currentOperationContext.ServiceSecurityContext;
            _loggerContextManager[LoggerContextKeys.Required.UserAccount] = securityServiceContext != null && !securityServiceContext.IsAnonymous
                                                            ? securityServiceContext.PrimaryIdentity.Name
                                                            : "Not available";
            
            _loggerContextManager[LoggerContextKeys.Optional.UserSession] = currentOperationContext.SessionId ?? "Not available";

            object remoteEndpointProperty;
            currentOperationContext.IncomingMessageProperties.TryGetValue(RemoteEndpointMessageProperty.Name, out remoteEndpointProperty);
            _loggerContextManager[LoggerContextKeys.Optional.UserAddress] = remoteEndpointProperty != null
                                                          ? ((RemoteEndpointMessageProperty)remoteEndpointProperty).Address
                                                          : "Not available";

            var currentwebOperationContext = WebOperationContext.Current;
            _loggerContextManager[LoggerContextKeys.Optional.UserAgent] = currentwebOperationContext != null && currentwebOperationContext.IncomingRequest.UserAgent != null
                                                               ? currentwebOperationContext.IncomingRequest.UserAgent
                                                               : "Not available";
        }
    }
}
