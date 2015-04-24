using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BL.Operations.Concrete.AdvertisementElements
{
    /// <summary>
    /// Уведомляет куратора об изменении файлового ЭРМ (а именно самого прикрепленного файла)
    /// </summary>
    public sealed class NotifyAboutAdvertisementElementFileChangedOperationService : INotifyAboutAdvertisementElementFileChangedOperationService
    {
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly ILinkToEntityCardFactory _linkToEntityCardFactory;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IUserReadModel _userReadModel;
        private readonly ISendNotificationOperationService _sendNotificationOperationService;

        public NotifyAboutAdvertisementElementFileChangedOperationService(
            IAdvertisementReadModel advertisementReadModel,
            ILinkToEntityCardFactory linkToEntityCardFactory,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory,
            IUserReadModel userReadModel,
            ISendNotificationOperationService sendNotificationOperationService)
        {
            _advertisementReadModel = advertisementReadModel;
            _linkToEntityCardFactory = linkToEntityCardFactory;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _userReadModel = userReadModel;
            _sendNotificationOperationService = sendNotificationOperationService;
        }

        public void Notify(long advertisementElementId)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<NotifyAboutAdvertisementElementFileChangedIdentity>())
            {
                var mailInfo = _advertisementReadModel.GetMailNotificationDto(advertisementElementId);
                if (mailInfo == null)
                {
                    scope.Complete();
                    return;
                }

                // Исключим из рассылки системных пользователей и текущего пользователя
                var orderOwnerCodes = _userReadModel.PickNonServiceUsers(mailInfo.OrderRefs.Select(x => x.OwnerCode).ToArray())
                                                    .Except(new[] { _userContext.Identity.Code })
                                                    .ToArray();

                if (!orderOwnerCodes.Any())
                {
                    scope.Complete();
                    return;
                }

                var subject = string.Format(BLResources.AdvertismentEditSubjectTemplate, mailInfo.FirmRef.Name);
                var message = string.Format(BLResources.AdvertismentEditBodyTemplate,
                                            _linkToEntityCardFactory
                                                .CreateLink<Firm>(mailInfo.FirmRef.Id.Value)
                                                .AsHtmlLinkWithTitle(mailInfo.FirmRef.Name),
                                            mailInfo.AdvertisementRef.Name,
                                            mailInfo.AdvertisementTemplateName);

                _sendNotificationOperationService.SendAsHtml(orderOwnerCodes, subject, message);

                scope.Complete();
            }
        }
    }
}