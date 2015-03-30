using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.NotificationEmail;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified
{
    public sealed class SendEmailNotificationOperationService : ISendNotificationOperationService
    {
        private readonly INotificationsSettings _notificationsSettings;
        private readonly INotificationSender _notificationSender;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public SendEmailNotificationOperationService(INotificationsSettings notificationsSettings, INotificationSender notificationSender, IEmployeeEmailResolver employeeEmailResolver, IOperationScopeFactory scopeFactory, ITracer tracer)
        {
            _notificationsSettings = notificationsSettings;
            _notificationSender = notificationSender;
            _employeeEmailResolver = employeeEmailResolver;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public void Send(IEnumerable<long> ownerCodes, string subject, string message)
        {
            if (!_notificationsSettings.EnableNotifications)
            {
                _tracer.Info("Notifications disabled in config file");
                return;
            }

            using (var scope = _scopeFactory.CreateNonCoupled<SendNotificationIdentity>())
            {
                foreach (var ownerCode in ownerCodes)
                {
                    string orderOwnerEmail;
                    if (_employeeEmailResolver.TryResolveEmail(ownerCode, out orderOwnerEmail) && !string.IsNullOrEmpty(orderOwnerEmail))
                    {
                        _notificationSender.PostMessage(new[] { new NotificationAddress(orderOwnerEmail) },
                                                        subject,
                                                        new NotificationBody { Body = message, IsHtml = true });
                    }
                    else
                    {
                        _tracer.Error("Can't send notification. Can't get email. Owner code: " + ownerCode);
                    }
                }

                scope.Complete();
            }
        }
    }
}
