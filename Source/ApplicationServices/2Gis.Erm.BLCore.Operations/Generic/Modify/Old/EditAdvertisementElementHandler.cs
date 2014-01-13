using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAdvertisementElementHandler : RequestHandler<EditAdvertisementElementRequest, EmptyResponse>
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IAppSettings _appSettings;
        private readonly ICommonLog _logger;
        private readonly INotificationSender _notificationSender;
        private readonly IEmployeeEmailResolver _employeeEmailResolver;
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IOrderValidationResultsResetter _orderValidationResultsResetter;
        private readonly IOperationScopeFactory _scopeFactory;
        readonly ILinkToEntityCardFactory _linkFactory;

        public EditAdvertisementElementHandler(IAppSettings appSettings,
            INotificationSender notificationSender,
            IEmployeeEmailResolver employeeEmailResolver,
            IUserContext userContext,
            ICommonLog logger,
            IAdvertisementRepository advertisementRepository,
            IFinder finder, 
            ISubRequestProcessor subRequestProcessor,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IOrderValidationResultsResetter orderValidationResultsResetter,
            IOperationScopeFactory scopeFactory,
            ILinkToEntityCardFactory linkFactory)
        {
            _appSettings = appSettings;
            _notificationSender = notificationSender;
            _employeeEmailResolver = employeeEmailResolver;
            _userContext = userContext;
            _logger = logger;
            _orderValidationResultsResetter = orderValidationResultsResetter;
            _scopeFactory = scopeFactory;
            _linkFactory = linkFactory;
            _advertisementRepository = advertisementRepository;
            _functionalAccessService = functionalAccessService;
            _finder = finder;
            _subRequestProcessor = subRequestProcessor;
        }

        protected override EmptyResponse Handle(EditAdvertisementElementRequest request)
        {
            var advertisementElement = request.Entity;

            PrepareFormattedText(request);
            PreparePlaneText(request);

            var dummyElements = new AdvertisementElement[] { };
            var ifAdvertisementPublished = _advertisementRepository.CheckIfAdvertisementPublished(advertisementElement.AdvertisementId);
            if (!ifAdvertisementPublished.IsPublished && !ifAdvertisementPublished.IsDummy)
            {
                throw new BusinessLogicException(BLResources.CantEditAdvertisementWithUnpublishedTemplate);
            }

            if (ifAdvertisementPublished.IsPublished && ifAdvertisementPublished.IsDummy)
            {
                throw new BusinessLogicException(BLResources.CantEditDummyAdvertisementWithPublishedTemplate);
            }

            if (ifAdvertisementPublished.IsDummy &&
                !_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.EditDummyAdvertisement, _userContext.Identity.Code))
            {
                throw new BusinessLogicException(BLResources.YouHaveNoPrivelegeToEditDummyAdvertisement);
            }

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(advertisementElement))
            {
                var validationInfo = _finder.Find<AdvertisementElement>(x => x.Id == advertisementElement.Id)
                                        .Select(x => new { x.AdvertisementElementTemplate.NeedsValidation, x.Status })
                                        .Single();

                if (validationInfo.NeedsValidation && !ifAdvertisementPublished.IsDummy)
                {
                    var previousStatus = (AdvertisementElementStatus)validationInfo.Status;
                    var newStatus = (AdvertisementElementStatus)advertisementElement.Status;

                    var response =
                        (AvailableStatesForAdvertisementElementResponse)
                            _subRequestProcessor.HandleSubRequest(new AvailableStatesForAdvertisementElementRequest { CurrentState = previousStatus }, Context);

                    if (!response.AvailableStates.Contains(newStatus))
                    {
                        throw new NotificationException(string.Format(BLResources.StatusTransitionIsNotAllowedForAdvElement,
                                                                      previousStatus.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture),
                                                                      newStatus.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture)));
                    }

                    var hasVerificationPrivilege = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementVerification, _userContext.Identity.Code);
                    
                    if (previousStatus != newStatus && !hasVerificationPrivilege)
                    {
                        throw new NotificationException(string.Format("{0} : {1}", BLResources.OperationNotAllowed, EnumResources.FunctionalPrivilegeNameAdvertisementVerification));
                    }

                    // Сбрасываем статус, если НЕ верификатор изменил контент РМ
                    if (IsContentChanged(advertisementElement, request) && !hasVerificationPrivilege)
                    {
                        advertisementElement.Status = (int)AdvertisementElementStatus.NotValidated;
                        advertisementElement.Error = (int)AdvertisementElementError.Absent;
                    }
                }

                _advertisementRepository.CreateOrUpdate(advertisementElement, request.PlainText, request.FormattedText, request.FileTimestamp);
                var isDummyAdvertisement = _advertisementRepository.IsAdvertisementDummy(advertisementElement.AdvertisementId);
                if (isDummyAdvertisement)
                {
                    dummyElements = _advertisementRepository.GetLinkedDummyElements(advertisementElement.AdvertisementElementTemplateId);
                    foreach (var dummyElement in dummyElements.Where(dummyElement => dummyElement.Id != advertisementElement.Id))
                    {
                        dummyElement.FileId = advertisementElement.FileId;
                        dummyElement.FasCommentType = advertisementElement.FasCommentType;
                        dummyElement.Text = advertisementElement.Text;
                        dummyElement.BeginDate = advertisementElement.BeginDate;
                        dummyElement.EndDate = advertisementElement.EndDate;
                        _advertisementRepository.CreateOrUpdate(dummyElement, request.PlainText, request.FormattedText, request.FileTimestamp);
                    }
                }

                // сбрасываем кеш проверок заказов
                var orderIds = _advertisementRepository.GetDependedOrderIds(new[] { advertisementElement.AdvertisementId });
                foreach (var orderId in orderIds)
                {
                    _orderValidationResultsResetter.SetGroupAsInvalid(orderId, OrderValidationRuleGroup.AdvertisementMaterialsValidation);
                }

                foreach (var dummyElement in dummyElements.Where(dummyElement => dummyElement.Id != advertisementElement.Id))
                {
                    orderIds = _advertisementRepository.GetDependedOrderIds(new[] { dummyElement.AdvertisementId });
                    foreach (var orderId in orderIds)
                    {
                        _orderValidationResultsResetter.SetGroupAsInvalid(orderId, OrderValidationRuleGroup.AdvertisementMaterialsValidation);
                    }
                }

                // уведомление об изменении в рекламном материале
                if (advertisementElement.FileId != null)
                {
                    var timestamp = _advertisementRepository.GetFileTimeStamp(advertisementElement.FileId.Value);
                    if (!string.Equals(timestamp, request.FileTimestamp, StringComparison.Ordinal))
                    {
                        NotifyOwnerAboutChanges(advertisementElement);
                    }
                }

                if (advertisementElement.Status == (int)AdvertisementElementStatus.Invalid)
                {
                    NotifyOwnerAboutInvalidAdvertisement(advertisementElement);
                }

                operationScope
                    .Updated<AdvertisementElement>(advertisementElement.Id)
                    .Complete();
            }

            return Response.Empty;
        }

        private static void PreparePlaneText(EditAdvertisementElementRequest request)
        {
            // cleanup linebreaks (Export does not recognize linebreaks)
            var plainText = request.PlainText;
            if (!string.IsNullOrEmpty(plainText))
            {
                plainText = plainText.Replace(Environment.NewLine, "\n");
                plainText = plainText.Replace((char)160, ' ');
            }

            request.PlainText = plainText;
        }

        private static void PrepareFormattedText(EditAdvertisementElementRequest request)
        {
            // делаем unescape на форматированный текст, т.к. до этого в js на него делаем escape
            // а escape в js мы сделали чтобы не передавать html в теле запроса, asp.net mvc ругается что это небезопасно
            var formattedText = request.FormattedText;
            if (!string.IsNullOrEmpty(formattedText))
            {
                // decode formatted text
                formattedText = Uri.UnescapeDataString(formattedText).Replace("\u001d", string.Empty);
                formattedText = Uri.UnescapeDataString(formattedText).Replace("&nbsp;", " ");
            }

            request.FormattedText = formattedText;
        }

        /// <summary>
        /// Уведомляет куратора об изменении ЭРМ
        /// </summary>
        /// <param name="advertisementElement"></param>
        private void NotifyOwnerAboutChanges(AdvertisementElement advertisementElement)
        {
            if (!CheckNotificationPossibility(advertisementElement.AdvertisementId))
            {
                return;
            }

            var changeInfo = _advertisementRepository.GetMailNotificationDto(advertisementElement.Id);
            
            if (changeInfo == null || changeInfo.FirmOwnerCode == _userContext.Identity.Code)
            {   // куратор заказа сам что-то поменял в рекламных материалах, нет необходимости в уведомлениях
                return;
            }

            var subject = string.Format(BLResources.AdvertismentEditSubjectTemplate, changeInfo.Firm.Name);
            var message = string.Format(
                BLResources.AdvertismentEditBodyTemplate,
                _linkFactory.CreateLinkTag(EntityName.Firm, changeInfo.Firm.Id, changeInfo.Firm.Name),
                changeInfo.Advertisement.Name,
                changeInfo.AdvertisementTemplateName);

            NotifyOwner(changeInfo, subject, message);
        }

        /// <summary>
        /// Уведомляет куратора о том, что модератор отклонил ЭРМ
        /// </summary>
        /// <param name="advertisementElement"></param>
        void NotifyOwnerAboutInvalidAdvertisement(AdvertisementElement advertisementElement)
        {
            if (!CheckNotificationPossibility(advertisementElement.AdvertisementId))
            {
                return;
            }

            var changeInfo = _advertisementRepository.GetMailNotificationDto(advertisementElement.Id);

            var subject = string.Format(BLResources.AdvertisementIsInvalidSubject);
            var message = string.Format(BLResources.AdvertisementIsInvalidMessage,
                                        _linkFactory.CreateLinkTag(EntityName.Advertisement, changeInfo.Advertisement.Id, changeInfo.Advertisement.Name),
                                        _linkFactory.CreateLinkTag(EntityName.Firm, changeInfo.Firm.Id, changeInfo.Firm.Name),
                                        changeInfo.AdvertisementElementTemplateName);

            if (changeInfo.Orders.Any())
            {
                var orderLinks = string.Join(", ", changeInfo.Orders.Select(order => _linkFactory.CreateLinkTag(EntityName.Order, order.Id, order.Name)));
                var messagePartTwo = string.Format(BLResources.OrdersWillNotBePublishedMessage, orderLinks);
                message = string.Format("{0} {1}", message, messagePartTwo);
            }

            NotifyOwner(changeInfo, subject, message);
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
                _logger.ErrorEx("Can't send notification about - advertisment element changed. Can't get to_address email. Firm name: " + changeInfo.Firm.Name +
                                ". Owner code: " + changeInfo.FirmOwnerCode);
            }
        }

        private bool CheckNotificationPossibility(long advertisementId)
        {
            if (!_appSettings.EnableNotifications)
            {
                _logger.InfoEx("Notifications disabled in config file");
                return false;
            }

            if (_advertisementRepository.CheckIfAdvertisementPublished(advertisementId).IsDummy)
            {
                // Отредактирована заглушка РМ. Уведомление не отправляем
                _logger.InfoEx("Отредактирована заглушка РМ. Уведомление не отправляем");
                return false;
            }

            return true;
        }

        private bool IsContentChanged(AdvertisementElement advertisementElement, EditAdvertisementElementRequest request)
        {
            var existingAdvertisementElement =
                _finder.Find<AdvertisementElement>(x => x.Id == advertisementElement.Id)
                       .Select(x => new
                           {
                               x.AdvertisementElementTemplate.RestrictionType, 
                               x.AdvertisementElementTemplate.FormattedText,
                               Element = x
                           })
                       .Single();

            var newTextValue = existingAdvertisementElement.FormattedText
                ? request.FormattedText
                : request.PlainText;

            switch ((AdvertisementElementRestrictionType)existingAdvertisementElement.RestrictionType)
            {
                case AdvertisementElementRestrictionType.Text:
                {
                    return existingAdvertisementElement.Element.Text != advertisementElement.Text
                        || existingAdvertisementElement.Element.Text != newTextValue;
                }

                case AdvertisementElementRestrictionType.Article:
                case AdvertisementElementRestrictionType.Image:
                {
                    // Обрабатывается только случай первичной загрузки файла (когда меняется FileId);
                    // случай загрузки файлов на место существующего обрабатывается в AdvertisementRepository.UploadFile
                    return existingAdvertisementElement.Element.FileId != advertisementElement.FileId;
                }

                case AdvertisementElementRestrictionType.FasComment:
                {
                    return advertisementElement.FasCommentType != (int)FasComment.NewFasComment
                        ? existingAdvertisementElement.Element.FasCommentType != advertisementElement.FasCommentType
                        : existingAdvertisementElement.Element.Text != newTextValue;
                }

                case AdvertisementElementRestrictionType.Date:
                {
                    return existingAdvertisementElement.Element.BeginDate != advertisementElement.BeginDate ||
                           existingAdvertisementElement.Element.EndDate != advertisementElement.EndDate;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
