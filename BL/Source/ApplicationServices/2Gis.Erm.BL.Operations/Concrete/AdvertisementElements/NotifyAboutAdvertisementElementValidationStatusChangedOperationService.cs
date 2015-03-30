using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

namespace DoubleGis.Erm.BL.Operations.Concrete.AdvertisementElements
{
    /// <summary>
    /// Уведомляет куратора о том, что модератор отклонил ЭРМ
    /// </summary>
    public sealed class NotifyAboutAdvertisementElementRejectionOperationService : INotifyAboutAdvertisementElementRejectionOperationService
    {
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly ILinkToEntityCardFactory _linkToEntityCardFactory;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISendNotificationOperationService _sendNotificationOperationService;
        private readonly IUserReadModel _userReadModel;
        private readonly IUserContext _userContext;

        public NotifyAboutAdvertisementElementRejectionOperationService(
            IAdvertisementReadModel advertisementReadModel,
            ILinkToEntityCardFactory linkToEntityCardFactory,
            IOperationScopeFactory scopeFactory,
            ISendNotificationOperationService sendNotificationOperationService,
            IUserReadModel userReadModel,
            IUserContext userContext)
        {
            _advertisementReadModel = advertisementReadModel;
            _linkToEntityCardFactory = linkToEntityCardFactory;
            _scopeFactory = scopeFactory;
            _sendNotificationOperationService = sendNotificationOperationService;
            _userReadModel = userReadModel;
            _userContext = userContext;
        }

        public void Notify(long advertisementElementId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<NotifyAboutAdvertisementElementRejectionIdentity>())
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

                // Исключим из рассылки системных пользователей и текущего пользователя
                var orderOwnerCodes = _userReadModel.PickNonServiceUsers(mailInfo.OrderRefs.Select(x => x.OwnerCode).ToArray())
                                                    .Except(new[] { _userContext.Identity.Code })
                                                    .ToArray();

                if (!orderOwnerCodes.Any())
                {
                    scope.Complete();
                    return;
                }

                var orderLinks =
                    string.Join(", ",
                                mailInfo.OrderRefs.Select(order => _linkToEntityCardFactory
                                                                       .CreateLink<Order>(order.Id)
                                                                       .AsHtmlLinkWithTitle(order.Number)));
                var messagePartTwo = string.Format(BLResources.OrdersWillNotBePublishedMessage, orderLinks);
                message = string.Format("{0} {1}", message, messagePartTwo);

                _sendNotificationOperationService.Send(orderOwnerCodes, subject, message);

                scope.Complete();
            }
        }
    }
}