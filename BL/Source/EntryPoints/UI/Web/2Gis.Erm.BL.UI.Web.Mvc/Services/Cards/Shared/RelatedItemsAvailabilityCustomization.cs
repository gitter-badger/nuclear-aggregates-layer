using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class RelatedItemsAvailabilityCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        public RelatedItemsAvailabilityCustomization(IMetadataProvider metadataProvider,
                                                        ISecurityServiceFunctionalAccess functionalAccessService,
                                                        ISecurityServiceEntityAccess entityAccessService,
                                                        IUserContext userContext)
        {
            _metadataProvider = metadataProvider;
            _functionalAccessService = functionalAccessService;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CardMetadata metadata;
            var metadataId = IdBuilder.For<MetadataCardsIdentity>(viewModel.ViewConfig.EntityName.ToString()).AsIdentity();
            if (!_metadataProvider.TryGetMetadata(metadataId.Id, out metadata))
            {
                throw new MetadataNotFoundException(metadataId);
            }

            if (metadata.HasRelatedItems)
            {
                foreach (var relatedItem in metadata.RelatedItems)
                {
                    ProcessRelatedItemMetadata(relatedItem, viewModel);
                }
            }
        }

        private void ProcessRelatedItemMetadata(UIElementMetadata relatedItemElementMetadata, IEntityViewModelBase model)
        {
            if (relatedItemElementMetadata.NameDescriptor == null)
            {
                return;

                // Или это не нормально и бросим исключение? Пока не ясно.
            }

            var elementName = relatedItemElementMetadata.NameDescriptor.ToString();

            // А является ли имя идентификатором?
            // Если не по имени, то как сопоставить элемент и его метаданные?
            var elementToEvaluate =
                model.ViewConfig
                     .CardSettings
                     .CardRelatedItems
                     .SelectMany(x => x.Items)
                     .SingleOrDefault(x => x.Name == elementName);

            if (elementToEvaluate == null)
            {
                return;
            }

            EvaluateRelatedItemAvailability(relatedItemElementMetadata, elementToEvaluate, model);
        }

        private void EvaluateRelatedItemAvailability(UIElementMetadata relatedItemElementMetadata, CardRelatedItemStructure relatedItemElement, IEntityViewModelBase model)
        {
            relatedItemElement.Disabled |= IsUIElementDisabled(relatedItemElementMetadata, model);

            if (IsUIElementInvisible(relatedItemElementMetadata, model))
            {
                model.ViewConfig.CardSettings.CardRelatedItems.SingleOrDefault().Items
                    = model.ViewConfig.CardSettings.CardRelatedItems.SingleOrDefault().Items.Except(new[] { relatedItemElement }).ToArray();
            }
        }

        private bool IsUIElementDisabled(UIElementMetadata element, IEntityViewModelBase model)
        {
            if (element.Uses<LockOnNewCardFeature>() && model.IsNew)
            {
                return true;
            }

            if (element.Uses<LockOnInactiveCardFeature>() && model.ViewConfig.ReadOnly)
            {
                return true;
            }

            if (element.Features<SecuredByFunctionalPrivelegeFeature>().Any(feature => !_functionalAccessService.HasFunctionalPrivilegeGranted(feature.Privilege, _userContext.Identity.Code)))
            {
                return true;
            }

            var modelOwnerCode = model.Owner != null && model.Owner.Key.HasValue
                                     ? model.Owner.Key.Value
                                     : _userContext.Identity.Code;

            foreach (var feature in element.Features<SecuredByEntityPrivelegeFeature>())
            {
                if (feature.Entity == model.ViewConfig.EntityName)
                {
                    if (!_entityAccessService.HasEntityAccess(feature.Privilege, feature.Entity, _userContext.Identity.Code, model.Id, modelOwnerCode, null))
                    {
                        return true;
                    }
                }
                else
                {
                    if (!_entityAccessService.HasEntityAccess(feature.Privilege, feature.Entity, _userContext.Identity.Code, null, _userContext.Identity.Code, null))
                    {
                        return true;
                    }
                }
            }

            foreach (var feature in element.Features<IDisableExpressionFeature>())
            {
                bool expressionResult;
                if (feature.TryExecute(model, out expressionResult) && expressionResult)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsUIElementInvisible<TViewModel>(UIElementMetadata element, TViewModel model)
            where TViewModel : IViewModelAbstract
        {
            foreach (var feature in element.Features<IHideExpressionFeature>())
            {
                bool expressionResult;
                if (feature.TryExecute(model, out expressionResult) && expressionResult)
                {
                    return true;
                }
            }

            return false;
        }
    }
}