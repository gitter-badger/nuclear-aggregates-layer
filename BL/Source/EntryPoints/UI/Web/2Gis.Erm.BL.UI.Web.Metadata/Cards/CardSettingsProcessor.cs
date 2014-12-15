using System;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards
{
    public sealed class CardSettingsProcessor : ICardSettingsProcessor
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly CultureInfo _currentCulture;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly long _currentUserCode; 

        public CardSettingsProcessor(IMetadataProvider metadataProvider, IUserContext userContext, ISecurityServiceFunctionalAccess functionalAccessService, ISecurityServiceEntityAccess entityAccessService)
        {
            _metadataProvider = metadataProvider;
            _functionalAccessService = functionalAccessService;
            _entityAccessService = entityAccessService;
            _currentCulture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
            _currentUserCode = userContext.Identity.Code;
        }

        public void ProcessCardSettings<TEntity, TViewModel>(TViewModel model)
            where TEntity : IEntity
            where TViewModel : IViewModelAbstract
        {
            CardMetadata metadata;
            if (!_metadataProvider.TryGetMetadata(IdBuilder.For<MetadataCardsIdentity>(typeof(TEntity).AsEntityName().ToString()).AsIdentity().Id, out metadata))
            {
                return;
            }

            var entityModel = model as IEntityViewModelBase;
            if (entityModel != null)
            {
                EvaluateIsReadOnly(entityModel, metadata);
                EvaluateTitle(entityModel, metadata);

                foreach (var actionElement in metadata.ActionsDescriptors)
                {
                    ProcessToolbarElementMetadata(actionElement, null, entityModel);
                }

                if (metadata.HasRelatedItems)
                {
                    foreach (var relatedItem in metadata.RelatedItems)
                    {
                        ProcessRelatedItemMetadata(relatedItem, entityModel);
                    }
                }
            }
        }

        internal void EvaluateIsReadOnly(IEntityViewModelBase entityModel, CardMetadata metadata)
        {
            entityModel.ViewConfig.ReadOnly |= metadata.Uses<ReadOnlyFeature>();
        }

        internal void EvaluateTitle(IEntityViewModelBase entityModel, CardMetadata metadata)
        {
            const string TitleTemplate = "{0} : {1}";

            if (string.IsNullOrWhiteSpace(entityModel.ViewConfig.CardSettings.Title))
            {
                entityModel.ViewConfig.CardSettings.Title = metadata.EntityLocalizationDescriptor != null
                                                                ? metadata.EntityLocalizationDescriptor.GetValue(_currentCulture)
                                                                : metadata.Entity.ToStringLocalized(EnumResources.ResourceManager, _currentCulture);

                if (entityModel.IsNew)
                {
                    entityModel.ViewConfig.CardSettings.Title = string.Format(TitleTemplate, entityModel.ViewConfig.CardSettings.Title, BLResources.Create);
                }
                else
                {
                    var mainAttributeFeature = metadata.Features<ICardMainAttributeFeature>().SingleOrDefault();
                    if (mainAttributeFeature == null)
                    {
                        return;
                    }

                    object mainAttributeValue;
                    if (mainAttributeFeature.PropertyDescriptor.TryGetValue(entityModel, out mainAttributeValue))
                    {
                        entityModel.ViewConfig.CardSettings.Title = string.Format(TitleTemplate, entityModel.ViewConfig.CardSettings.Title, mainAttributeValue);
                    }
                }
            }
        }

        internal void ProcessToolbarElementMetadata(UiElementMetadata toolbarElement, UiElementMetadata parentElement, IEntityViewModelBase model)
        {
            foreach (var childElement in toolbarElement.Elements.OfType<UiElementMetadata>())
            {
                ProcessToolbarElementMetadata(childElement, toolbarElement, model);
            }

            if (toolbarElement.NameDescriptor == null)
            {
                return;
                // Или это не нормально и бросим исключение? Пока не ясно.
            }

            var elementName = toolbarElement.NameDescriptor.ToString();
            if (elementName == "Splitter")
            {
                return;
            }
            var parentName = string.Empty;
            if (parentElement != null && parentElement.NameDescriptor != null)
            {
                parentName = parentElement.NameDescriptor.ToString();
            }

            // А является ли имя идентификатором? Например, splitter'ов у нас несколько
            // Если не по имени, то как сопоставить элемент и его метаданные?
            var elementToEvaluate =
                model
                    .ViewConfig
                    .CardSettings
                    .CardToolbar
                    .SingleOrDefault(x => x.Name == elementName && (parentName == string.Empty || x.ParentName == parentName));

            if (elementToEvaluate == null)
            {
                return;
            }

            EvaluateToolbarElementAvailability(toolbarElement, elementToEvaluate, model);
        }

        internal void EvaluateToolbarElementAvailability<TViewModel>(UiElementMetadata toolbarElementMetadata, ToolbarElementStructure toolbarlement, TViewModel model)
            where TViewModel : IEntityViewModelBase
        {
            toolbarlement.Disabled |= IsUiElementDisabled(toolbarElementMetadata, model);

            if (IsUiElementInvisible(toolbarElementMetadata, model))
            {
                model.ViewConfig.CardSettings.CardToolbar
                    = model.ViewConfig.CardSettings.CardToolbar.Except(new[] { toolbarlement }).ToArray();
            }
        }

        internal void ProcessRelatedItemMetadata(UiElementMetadata relatedItemElementMetadata, IEntityViewModelBase model)
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
            EvaluateRelatedItemExtendedInfo(relatedItemElementMetadata, elementToEvaluate, model);
        }

        internal void EvaluateRelatedItemAvailability(UiElementMetadata relatedItemElementMetadata, CardRelatedItemStructure relatedItemElement, IEntityViewModelBase model)
        {
            relatedItemElement.Disabled |= IsUiElementDisabled(relatedItemElementMetadata, model);

            if (IsUiElementInvisible(relatedItemElementMetadata, model))
            {
                model.ViewConfig.CardSettings.CardRelatedItems.SingleOrDefault().Items
                    = model.ViewConfig.CardSettings.CardRelatedItems.SingleOrDefault().Items.Except(new[] { relatedItemElement }).ToArray();
            }
        }

        internal void EvaluateRelatedItemExtendedInfo(UiElementMetadata relatedItemElementMetadata, CardRelatedItemStructure relatedItemElement, IEntityViewModelBase model)
        {
            foreach (var extendedInfoFeature in relatedItemElementMetadata.Features.OfType<ExtendedInfoFeature>())
            {
                var templatedExtendedInfo = extendedInfoFeature.ExtendedInfo as TemplateDescriptor;

                if (templatedExtendedInfo != null)
                {
                    string extendedInfo;
                    if (!templatedExtendedInfo.TryFormat(_currentCulture, out extendedInfo, model))
                    {
                        throw new InvalidOperationException(string.Format("Unable to determine extendedInfo for {0} related item", relatedItemElement.Name));
                    }

                    relatedItemElement.AppendExtendedInfo(extendedInfo);
                }
                else
                {
                    relatedItemElement.AppendExtendedInfo(extendedInfoFeature.ExtendedInfo.ToString());
                }
            }
        }

        internal bool IsUiElementDisabled(UiElementMetadata element, IEntityViewModelBase model)
        {
            if (element.Uses<LockOnNewCardFeature>() && model.IsNew)
            {
                return true;
            }

            if (element.Uses<LockOnInactiveCardFeature>() && model.ViewConfig.ReadOnly)
            {
                return true;
            }

            if (element.Features<SecuredByFunctionalPrivelegeFeature>().Any(feature => !_functionalAccessService.HasFunctionalPrivilegeGranted(feature.Privilege, _currentUserCode)))
            {
                return true;
            }

            var modelOwnerCode = model.Owner != null && model.Owner.Key.HasValue
                                     ? model.Owner.Key.Value
                                     : _currentUserCode;

            foreach (var feature in element.Features<SecuredByEntityPrivelegeFeature>())
            {
                if (feature.Entity == model.ViewConfig.EntityName)
                {
                    if (!_entityAccessService.HasEntityAccess(feature.Privilege, feature.Entity, _currentUserCode, model.Id, modelOwnerCode, null))
                    {
                        return true;
                    }
                }
                else
                {
                    if (!_entityAccessService.HasEntityAccess(feature.Privilege, feature.Entity, _currentUserCode, null, _currentUserCode, null))
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

        internal bool IsUiElementInvisible<TViewModel>(UiElementMetadata element, TViewModel model)
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
