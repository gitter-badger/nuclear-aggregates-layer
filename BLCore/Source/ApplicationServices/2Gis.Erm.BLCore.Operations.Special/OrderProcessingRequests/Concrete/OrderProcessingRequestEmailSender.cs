using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Services.Notifications;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Security;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Special.OrderProcessingRequests.Concrete
{
    public class OrderProcessingRequestEmailSender : IOrderProcessingRequestEmailSender,
                                                     ICreatedOrderProcessingRequestEmailSender
    {
        private readonly ICommonLog _logger;
        private readonly ISecurityServiceUserIdentifier _userIdentityService;
        private readonly INotificationSender _notificationSender;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly INotificationsSettings _notificationsSettings;
        private readonly IOrderProcessingRequestNotificationFormatter _notificationFormatter;

        public OrderProcessingRequestEmailSender(INotificationsSettings notificationsSettings,
                                                 INotificationSender notificationSender,
                                                 IEmployeeEmailResolver employeeEmailResolver,
                                                 IOrderProcessingRequestNotificationFormatter notificationFormatter,
                                                 ISecurityServiceUserIdentifier userIdentityService,
                                                 ICommonLog logger)
        {
            _notificationsSettings = notificationsSettings;
            _notificationFormatter = notificationFormatter;
            _notificationSender = notificationSender;
            _employeeEmailResolver = employeeEmailResolver;
            _userIdentityService = userIdentityService;
            _logger = logger;
        }

        public OrderProcessingRequestEmailSendResult SendProcessingMessages(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest,
                                        IEnumerable<IMessageWithType> messagesToSend)
        {
            if (!EnsureNotificationIsEnabled())
            {
                var result = new OrderProcessingRequestEmailSendResult();
                result.Errors.Add(BLResources.MessagesNotSendedBecauseNotificationIsDisabled);
                return result;
            }

            // в Bootstrapper для TaskService указана настоящая реализация IOrderProcessingRequestNotificationHelpersFactory,
            // а для MVC - NULL-овая. Таким образом при ручном продлении заказа по ЛК, письмо должно не отправляться
            var message = _notificationFormatter.Format(orderProcessingRequest, messagesToSend);
            return TrySendEmail(orderProcessingRequest, message);
        }

        public OrderProcessingRequestEmailSendResult SendRequestIsCreatedMessage(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest)
        {
            // TODO {all, 18.12.2013}: После разделения NotificationFormatter необходимо убрать вызов метода SendProcessingMessages(orderProcessingRequest, Enumerable.Empty<IMessageWithType>());
            return SendProcessingMessages(orderProcessingRequest, Enumerable.Empty<IMessageWithType>());
        }

        private OrderProcessingRequestEmailSendResult TrySendEmail(Platform.Model.Entities.Erm.OrderProcessingRequest orderProcessingRequest,
                                                                   NotificationMessage message)
        {
            var result = new OrderProcessingRequestEmailSendResult();

            string email;
            var recieverCode = orderProcessingRequest.OwnerCode;
            if (!TryResolveEmailAddress(recieverCode, out email))
            {
                var userInfo = _userIdentityService.GetUserInfo(recieverCode);

                result.Errors.Add(string.Format(BLResources.OrderProcessingRequestSendNotificationErrorEmailMissingTemplate, userInfo.DisplayName));
            }
            else
            {
            _notificationSender.PostMessage(new[] { new NotificationAddress(email) }, message.Subject, message.Body);
            }

            return result;
        }

        private bool TryResolveEmailAddress(long userCode, out string email)
        {
            return _employeeEmailResolver.TryResolveEmail(userCode, out email);
        }

        private bool EnsureNotificationIsEnabled()
        {
            if (!_notificationsSettings.EnableNotifications)
            {
                _logger.Info("Notifications disabled in config file");
                return false;
            }

            return true;
        }
    }
}
