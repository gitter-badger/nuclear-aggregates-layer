using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements
{
    // TODO {i.maslennikov, 04.03.2014}: Этот сервис очень логично будет передать в компонент BL
    /// <summary>
    /// Уведомляет куратора о том, что модератор отклонил ЭРМ
    /// </summary>
    public sealed class NotifyAboutAdvertisementElementValidationStatusChangedOperationService : INotifyAboutAdvertisementElementValidationStatusChangedOperationService
    {
        private readonly INotificationsSettings _notificationsSettings;
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly INotificationSender _notificationSender;
        private readonly ILinkToEntityCardFactory _linkToEntityCardFactory;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public NotifyAboutAdvertisementElementValidationStatusChangedOperationService(
            INotificationsSettings notificationsSettings, 
            IAdvertisementReadModel advertisementReadModel,
            INotificationSender notificationSender,
            ILinkToEntityCardFactory linkToEntityCardFactory,
            IEmployeeEmailResolver employeeEmailResolver,
            IOperationScopeFactory scopeFactory,
            ITracer tracer)
        {
            _notificationsSettings = notificationsSettings;
            _advertisementReadModel = advertisementReadModel;
            _notificationSender = notificationSender;
            _linkToEntityCardFactory = linkToEntityCardFactory;
            _employeeEmailResolver = employeeEmailResolver;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public void Notify(long advertisementElementId)
        {
            if (!_notificationsSettings.EnableNotifications)
            {
                _tracer.Info("Notifications disabled in config file");
                return;
            }

            using (var scope = _scopeFactory.CreateNonCoupled<NotifyAboutAdvertisementElementValidationStatusChangedIdentity>())
            {
                var mailInfo = _advertisementReadModel.GetMailNotificationDto(advertisementElementId);

                var subject = string.Format(BLResources.AdvertisementIsInvalidSubject);
                var message = string.Format(BLResources.AdvertisementIsInvalidMessage,
                                            _linkToEntityCardFactory
                                                .CreateLink<Advertisement>(mailInfo.AdvertisementRef.Id.Value)
                                                .AsHtmlLinkWithTitle(mailInfo.AdvertisementRef.Name),
                                            _linkToEntityCardFactory
                                                .CreateLink<Firm>(mailInfo.FirmRef.Id.Value)
                                                .AsHtmlLinkWithTitle(mailInfo.FirmRef.Name),
                                            mailInfo.AdvertisementElementTemplateName);

                if (mailInfo.OrderRefs.Any())
                {
                    var orderLinks =
                        string.Join(", ",
                                    mailInfo.OrderRefs.Select(order => _linkToEntityCardFactory
                                                                           .CreateLink<Order>(order.Id.Value)
                                                                           .AsHtmlLinkWithTitle(order.Name)));
                    var messagePartTwo = string.Format(BLResources.OrdersWillNotBePublishedMessage, orderLinks);
                    message = string.Format("{0} {1}", message, messagePartTwo);
                }

                NotifyOwner(mailInfo, subject, message);

                scope.Complete();
            }
        }

        private void NotifyOwner(AdvertisementMailNotificationDto changeInfo, string subject, string message)
        {
            string orderOwnerEmail;
            if (_employeeEmailResolver.TryResolveEmail(changeInfo.FirmOwnerCode, out orderOwnerEmail) && !string.IsNullOrEmpty(orderOwnerEmail))
            {
                _notificationSender.PostMessage(new[] { new NotificationAddress(orderOwnerEmail) },
                                                subject,
                                                new NotificationBody { Body = message, IsHtml = true });
            }
            else
            {
                _tracer.Error("Can't send notification about - advertisment element changed. Can't get to_address email. Firm name: " + changeInfo.FirmRef.Name +
                                ". Owner code: " + changeInfo.FirmOwnerCode);
            }
        }
    }
}