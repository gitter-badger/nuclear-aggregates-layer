using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements
{
    // TODO {i.maslennikov, 04.03.2014}: Этот сервис очень логично будет передать в компонент BL
    /// <summary>
    /// Уведомляет куратора об изменении файлового ЭРМ (а именно самого прикрепленного файла)
    /// </summary>
    public sealed class NotifyAboutAdvertisementElementFileChangedOperationService : INotifyAboutAdvertisementElementFileChangedOperationService
    {
        private readonly INotificationsSettings _notificationsSettings;
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly INotificationSender _notificationSender;
        private readonly ILinkToEntityCardFactory _linkToEntityCardFactory;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _logger;

        public NotifyAboutAdvertisementElementFileChangedOperationService(
            INotificationsSettings notificationsSettings, 
            IAdvertisementReadModel advertisementReadModel,
            INotificationSender notificationSender,
            ILinkToEntityCardFactory linkToEntityCardFactory,
            IEmployeeEmailResolver employeeEmailResolver,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory,
            ITracer logger)
        {
            _notificationsSettings = notificationsSettings;
            _advertisementReadModel = advertisementReadModel;
            _notificationSender = notificationSender;
            _linkToEntityCardFactory = linkToEntityCardFactory;
            _employeeEmailResolver = employeeEmailResolver;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Notify(long advertisementElementId)
        {
            if (!_notificationsSettings.EnableNotifications)
            {
                _logger.Info("Notifications disabled in config file");
                return;
            }

            using (var scope = _scopeFactory.CreateNonCoupled<NotifyAboutAdvertisementElementFileChangedIdentity>())
            {
                var mailInfo = _advertisementReadModel.GetMailNotificationDto(advertisementElementId);
                if (mailInfo == null || mailInfo.FirmOwnerCode == _userContext.Identity.Code)
                {
                    // куратор фирмы сам что-то поменял в рекламных материалах, нет необходимости в уведомлениях
                    scope.Complete();
                    return;
                }

                var subject = string.Format(BLResources.AdvertismentEditSubjectTemplate, mailInfo.FirmRef.Name);
                var message = string.Format(
                    BLResources.AdvertismentEditBodyTemplate,
                    _linkToEntityCardFactory
                        .CreateLink<Firm>(mailInfo.FirmRef.Id.Value)
                        .AsHtmlLinkWithTitle(mailInfo.FirmRef.Name),
                    mailInfo.AdvertisementRef.Name,
                    mailInfo.AdvertisementTemplateName);

                NotifyOwner(mailInfo, subject, message);

                scope.Complete();
            }
        }

        private void NotifyOwner(AdvertisementMailNotificationDto mailInfo, string subject, string message)
        {
            string orderOwnerEmail;
            if (_employeeEmailResolver.TryResolveEmail(mailInfo.FirmOwnerCode, out orderOwnerEmail) && !string.IsNullOrEmpty(orderOwnerEmail))
            {
                _notificationSender.PostMessage(new[] { new NotificationAddress(orderOwnerEmail) },
                                                subject,
                                                new NotificationBody { Body = message, IsHtml = true });
            }
            else
            {
                _logger.Error("Can't send notification about - advertisment element changed. Can't get to_address email. Firm name: " + mailInfo.FirmRef.Name +
                                ". Owner code: " + mailInfo.FirmOwnerCode);
            }
        }
    }
}
