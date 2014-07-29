using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AdvertisementElements;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Update.AdvertisementElements
{
    public sealed class UpdateAdvertisementElementOperationService : UpdateOperationServiceBase<AdvertisementElement, AdvertisementElementDomainEntityDto>
    {
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly INotifyAboutAdvertisementElementValidationStatusChangedOperationService _notifyAboutAdvertisementElementValidationStatusChangedOperationService;
        private readonly IAdvertisementUpdateElementAggregateService _advertisementUpdateElementAggregateService;
        private readonly IModifyingAdvertisementElementValidator _modifyingAdvertisementElementValidator;
        private readonly IOrderValidationInvalidator _orderValidationInvalidator;
        private readonly IAdvertisementElementPlainTextHarmonizer _advertisementElementPlainTextHarmonizer;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;

        public UpdateAdvertisementElementOperationService(
            IAdvertisementReadModel advertisementReadModel,
            INotifyAboutAdvertisementElementValidationStatusChangedOperationService notifyAboutAdvertisementElementValidationStatusChangedOperationService,
            IAdvertisementUpdateElementAggregateService advertisementUpdateElementAggregateService,
            IBusinessModelEntityObtainer<AdvertisementElement> entityObtainer,
            IModifyingAdvertisementElementValidator modifyingAdvertisementElementValidator,
            IOrderValidationInvalidator orderValidationInvalidator,
            IAdvertisementElementPlainTextHarmonizer advertisementElementPlainTextHarmonizer,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory) 
            : base(entityObtainer)
        {
            _advertisementReadModel = advertisementReadModel;
            _notifyAboutAdvertisementElementValidationStatusChangedOperationService = notifyAboutAdvertisementElementValidationStatusChangedOperationService;
            _advertisementUpdateElementAggregateService = advertisementUpdateElementAggregateService;
            _modifyingAdvertisementElementValidator = modifyingAdvertisementElementValidator;
            _orderValidationInvalidator = orderValidationInvalidator;
            _advertisementElementPlainTextHarmonizer = advertisementElementPlainTextHarmonizer;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
        }

        protected override void Update(AdvertisementElement entity, AdvertisementElementDomainEntityDto entityDto)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, AdvertisementElement>())
            {
                var advertisementInfo = _advertisementReadModel.GetAdvertisementInfoForElement(entity.Id);
                var plainTextHarmonized = _advertisementElementPlainTextHarmonizer.Process(entityDto.PlainText);

                if (!advertisementInfo.IsDummy)
                {
                    UpdateRegularElement(advertisementInfo, entity, plainTextHarmonized, entityDto.FormattedText);
                }
                else
                {
                    UpdateDummyElement(advertisementInfo, entity, plainTextHarmonized, entityDto.FormattedText);
                }

                scope.Complete();
            }
        }

        private void UpdateRegularElement(
            AdvertisementElementModifyDto advertisementElementInfo,
            AdvertisementElement entity,
            string plainText,
            string formattedText)
        {
            if (!advertisementElementInfo.IsPublished)
            {
                // TODO {i.maslennikov, 04.03.2014}: Не в контексте задачи ERM-3410. Предлагаю выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                throw new BusinessLogicException(BLResources.CantEditAdvertisementWithUnpublishedTemplate);
            }

            if (advertisementElementInfo.ElementTemplate.NeedsValidation)
            {
                var targetStatus = (AdvertisementElementStatus)entity.Status;
                if (!AdmissibleStates(advertisementElementInfo.PreviousStatusElementStatus).Contains(targetStatus))
                {
                    // TODO {i.maslennikov, 04.03.2014}: Не в контексте задачи ERM-3410. Предлагаю выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                    throw new BusinessLogicException(string.Format(BLResources.StatusTransitionIsNotAllowedForAdvElement,
                                                                  advertisementElementInfo.PreviousStatusElementStatus.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture),
                                                                  targetStatus.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture)));
                }

                var hasVerificationPrivilege = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementVerification, _userContext.Identity.Code);
                if (advertisementElementInfo.PreviousStatusElementStatus != targetStatus && !hasVerificationPrivilege)
                {
                    // TODO {i.maslennikov, 04.03.2014}: Не в контексте задачи ERM-3410. Предлагаю выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                    throw new BusinessLogicException(string.Format("{0} : {1}", BLResources.OperationNotAllowed, EnumResources.FunctionalPrivilegeNameAdvertisementVerification));
                }

                if (IsContentChanged(advertisementElementInfo, entity, plainText, formattedText) && !hasVerificationPrivilege)
                {   // Сбрасываем статус, если НЕ верификатор изменил контент РМ
                    entity.Status = (int)AdvertisementElementStatus.NotValidated;
                    entity.Error = (int)AdvertisementElementError.Absent;
                }
            }

            _modifyingAdvertisementElementValidator.Validate(advertisementElementInfo.ElementTemplate, 
                                                            new[] { entity },
                                                            plainText,
                                                            formattedText);

            _advertisementUpdateElementAggregateService.Update(new[] { entity }, advertisementElementInfo.ElementTemplate, plainText, formattedText);

            var orderIds = _advertisementReadModel.GetDependedOrderIds(new[] { entity.AdvertisementId });
            _orderValidationInvalidator.Invalidate(orderIds, OrderValidationRuleGroup.AdvertisementMaterialsValidation);

            if (entity.Status == (int)AdvertisementElementStatus.Invalid)
            {
                _notifyAboutAdvertisementElementValidationStatusChangedOperationService.Notify(entity.Id);
            }
        }

        private void UpdateDummyElement(
            AdvertisementElementModifyDto advertisementElementInfo,
            AdvertisementElement entity,
            string plainText,
            string formattedText)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.EditDummyAdvertisement, _userContext.Identity.Code))
            {
                // TODO {i.maslennikov, 04.03.2014}: Не в контексте задачи ERM-3410. Предлагаю выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                throw new BusinessLogicException(BLResources.YouHaveNoPrivelegeToEditDummyAdvertisement);
            }
            
            if (advertisementElementInfo.IsPublished)
            {
                // TODO {i.maslennikov, 04.03.2014}: Не в контексте задачи ERM-3410. Предлагаю выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                throw new BusinessLogicException(BLResources.CantEditDummyAdvertisementWithPublishedTemplate);
            }

            var allRelatedDummies = new List<AdvertisementElement> { entity };
            foreach (var dummyElement in advertisementElementInfo.ClonedDummies)
            {
                dummyElement.FileId = entity.FileId;
                dummyElement.FasCommentType = entity.FasCommentType;
                dummyElement.Text = entity.Text;
                dummyElement.BeginDate = entity.BeginDate;
                dummyElement.EndDate = entity.EndDate;
                allRelatedDummies.Add(dummyElement);
            }

            _modifyingAdvertisementElementValidator.Validate(advertisementElementInfo.ElementTemplate,
                                                            allRelatedDummies,
                                                            plainText,
                                                            formattedText);

            _advertisementUpdateElementAggregateService.Update(allRelatedDummies, advertisementElementInfo.ElementTemplate, plainText, formattedText);

            var orderIds = _advertisementReadModel.GetDependedOrderIds(allRelatedDummies.Select(x => x.AdvertisementId));
            _orderValidationInvalidator.Invalidate(orderIds, OrderValidationRuleGroup.AdvertisementMaterialsValidation);
        }

        private bool IsContentChanged(
            AdvertisementElementModifyDto advertisementElementInfo,
            AdvertisementElement entity,
            string plainText,
            string formattedText)
        {
            var newTextValue = advertisementElementInfo.ElementTemplate.FormattedText
                ? formattedText
                : plainText;

            switch ((AdvertisementElementRestrictionType)advertisementElementInfo.ElementTemplate.RestrictionType)
            {
                case AdvertisementElementRestrictionType.Text:
                {
                    return advertisementElementInfo.Element.Text != entity.Text
                        || advertisementElementInfo.Element.Text != newTextValue;
                }
                case AdvertisementElementRestrictionType.Article:
                case AdvertisementElementRestrictionType.Image:
                {
                    // Обрабатывается только случай первичной загрузки файла (когда меняется FileId);
                    // случай загрузки файлов на место существующего обрабатывается в AdvertisementRepository.UploadFile
                    return advertisementElementInfo.Element.FileId != entity.FileId;
                }
                case AdvertisementElementRestrictionType.FasComment:
                {
                    return entity.FasCommentType != (int)FasComment.NewFasComment
                        ? advertisementElementInfo.Element.FasCommentType != entity.FasCommentType
                        : advertisementElementInfo.Element.Text != newTextValue;
                }
                case AdvertisementElementRestrictionType.Date:
                {
                    return advertisementElementInfo.Element.BeginDate != entity.BeginDate ||
                            advertisementElementInfo.Element.EndDate != entity.EndDate;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<AdvertisementElementStatus> AdmissibleStates(AdvertisementElementStatus previousStatus)
        {
            switch (previousStatus)
            {
                case AdvertisementElementStatus.NotValidated:
                    return new[] 
                        { 
                            AdvertisementElementStatus.NotValidated,
                            AdvertisementElementStatus.Valid,
                            AdvertisementElementStatus.Invalid
                        };
                case AdvertisementElementStatus.Valid:
                case AdvertisementElementStatus.Invalid:
                    return new[]
                        {
                            AdvertisementElementStatus.Valid,
                            AdvertisementElementStatus.Invalid
                        };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}