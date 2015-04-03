using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Update;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Operations.Crosscutting.AdvertisementElements;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

using OrderValidationRuleGroup = DoubleGis.Erm.BLCore.API.OrderValidation.OrderValidationRuleGroup;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Update.AdvertisementElements
{
    public sealed class UpdateAdvertisementElementOperationService : IUpdateOperationService<AdvertisementElement>
    {
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly IAdvertisementUpdateElementAggregateService _advertisementUpdateElementAggregateService;
        private readonly IModifyingAdvertisementElementValidator _modifyingAdvertisementElementValidator;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;
        private readonly IAdvertisementElementPlainTextHarmonizer _advertisementElementPlainTextHarmonizer;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IBusinessModelEntityObtainer<AdvertisementElement> _entityObtainer;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;

        public UpdateAdvertisementElementOperationService(
            IAdvertisementReadModel advertisementReadModel,
            IAdvertisementUpdateElementAggregateService advertisementUpdateElementAggregateService,
            IBusinessModelEntityObtainer<AdvertisementElement> entityObtainer,
            IModifyingAdvertisementElementValidator modifyingAdvertisementElementValidator,
            IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService,
            IAdvertisementElementPlainTextHarmonizer advertisementElementPlainTextHarmonizer,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory)
        {
            _entityObtainer = entityObtainer;
            _advertisementReadModel = advertisementReadModel;
            _advertisementUpdateElementAggregateService = advertisementUpdateElementAggregateService;
            _modifyingAdvertisementElementValidator = modifyingAdvertisementElementValidator;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
            _advertisementElementPlainTextHarmonizer = advertisementElementPlainTextHarmonizer;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
        }

        public void Update(IDomainEntityDto entityDto)
        {
            var dto = (AdvertisementElementDomainEntityDto)entityDto;
            using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, AdvertisementElement>())
            {
                var updatedAdvertisementElement = _entityObtainer.ObtainBusinessModelEntity(entityDto);

                var advertisementInfo = _advertisementReadModel.GetAdvertisementInfoForElement(updatedAdvertisementElement.Id);
                var plainTextHarmonized = _advertisementElementPlainTextHarmonizer.Process(dto.PlainText);

                var orderIds = !advertisementInfo.IsDummy
                                   ? UpdateRegularElement(advertisementInfo, updatedAdvertisementElement, plainTextHarmonized, dto.FormattedText)
                                   : UpdateDummyElement(advertisementInfo, updatedAdvertisementElement, plainTextHarmonized, dto.FormattedText);

                _registerOrderStateChangesOperationService.Changed(orderIds.Select(x => new OrderChangesDescriptor
                                                                                            {
                                                                                                OrderId = x,
                                                                                                ChangedAspects =
                                                                                                    new[]
                                                                                                        {
                                                                                                            OrderValidationRuleGroup
                                                                                                                .AdvertisementMaterialsValidation
                                                                                                        }
                                                                                            }));

                scope.Complete();
            }
        }

        private IEnumerable<long> UpdateRegularElement(AdvertisementElementModifyDto advertisementElementInfo,
                                                       AdvertisementElement entity,
                                                       string plainText,
                                                       string formattedText)
        {
            if (!advertisementElementInfo.IsPublished)
            {
                // TODO {all, 04.03.2014}: Выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                throw new BusinessLogicException(BLResources.CantEditAdvertisementWithUnpublishedTemplate);
            }

            _modifyingAdvertisementElementValidator.Validate(advertisementElementInfo.ElementTemplate,
                                                             new[] { entity },
                                                             plainText,
                                                             formattedText);

            _advertisementUpdateElementAggregateService.Update(new[] { entity }, advertisementElementInfo.ElementTemplate, plainText, formattedText);

            return _advertisementReadModel.GetDependedOrderIds(new[] { entity.AdvertisementId });
        }

        private IEnumerable<long> UpdateDummyElement(AdvertisementElementModifyDto advertisementElementInfo,
                                                     AdvertisementElement entity,
                                                     string plainText,
                                                     string formattedText)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.EditDummyAdvertisement, _userContext.Identity.Code))
            {
                // TODO {all, 04.03.2014}: Выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                throw new BusinessLogicException(BLResources.YouHaveNoPrivelegeToEditDummyAdvertisement);
            }

            if (advertisementElementInfo.IsPublished)
            {
                // TODO {all, 04.03.2014}: Выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
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

            return _advertisementReadModel.GetDependedOrderIds(allRelatedDummies.Select(x => x.AdvertisementId));
        }
    }
}