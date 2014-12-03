using System.ComponentModel;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
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
                EvaluateTitle(entityModel, metadata);

                foreach (var actionElement in metadata.ActionsDescriptors)
                {
                    EvaluateToolbarElementAvailability(actionElement, null, model);
                }
            }
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
                    var mainAttributeFeature = metadata.Features<CardMainAttributeFeature>().SingleOrDefault();
                    if (mainAttributeFeature == null)
                    {
                        return;
                    }

                    var value = TypeDescriptor.GetProperties(entityModel)[mainAttributeFeature.Property.PropertyName].GetValue(entityModel);
                    if (value != null)
                    {
                        var mainAttributeValue = value.ToString();
                        entityModel.ViewConfig.CardSettings.Title = string.Format(TitleTemplate, entityModel.ViewConfig.CardSettings.Title, mainAttributeValue);
                    }
                }
            }
        }

        internal void EvaluateToolbarElementAvailability<TViewModel>(UiElementMetadata toolbarElement, UiElementMetadata parentElement, TViewModel model)
            where TViewModel : IViewModelAbstract
        {
            var entityModel = model as IEntityViewModelBase;
            foreach (var childElement in toolbarElement.Elements.OfType<UiElementMetadata>())
            {
                EvaluateToolbarElementAvailability(childElement, toolbarElement, model);
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
                entityModel
                    .ViewConfig
                    .CardSettings
                    .CardToolbar
                    .SingleOrDefault(x => x.Name == elementName && (parentName == string.Empty || x.ParentName == parentName));

            if (elementToEvaluate == null)
            {
                return;
            }

            if (toolbarElement.Uses<LockOnNewCardFeature>() && ((IEntityViewModelBase)model).IsNew)
            {
                elementToEvaluate.Disabled = true;
            }

            if (toolbarElement.Uses<LockOnInactiveCardFeature>() && ((IEntityViewModelBase)model).ViewConfig.ReadOnly)
            {
                elementToEvaluate.Disabled = true;
            }

            foreach (var feature in toolbarElement.Features<SecuredByFunctionalPrivelegeFeature>())
            {
                elementToEvaluate.Disabled |= !_functionalAccessService.HasFunctionalPrivilegeGranted(feature.Privilege, _currentUserCode);
            }

            var modelOwnerCode = entityModel.Owner.Key.HasValue ? entityModel.Owner.Key.Value : _currentUserCode;
            foreach (var feature in toolbarElement.Features<SecuredByEntityPrivelegeFeature>())
            {
                if (feature.Entity == entityModel.ViewConfig.EntityName)
                {
                    elementToEvaluate.Disabled |=
                        !_entityAccessService.HasEntityAccess(feature.Privilege, feature.Entity, _currentUserCode, entityModel.Id, modelOwnerCode, null);
                }
                else
                {
                    elementToEvaluate.Disabled |=
                        !_entityAccessService.HasEntityAccess(feature.Privilege, feature.Entity, _currentUserCode, null, _currentUserCode, null);
                }
            }

            foreach (var feature in toolbarElement.Features<IDisableExpressionFeature>())
            {
                bool expressionResult;
                if (feature.TryExecute(model, out expressionResult))
                {
                    elementToEvaluate.Disabled |= expressionResult;
                }
            }

            foreach (var feature in toolbarElement.Features<IHideExpressionFeature>())
            {
                bool expressionResult;
                if (feature.TryExecute(model, out expressionResult) && expressionResult)
                {
                    entityModel.ViewConfig.CardSettings.CardToolbar
                        = entityModel.ViewConfig.CardSettings.CardToolbar.Except(new[] { elementToEvaluate }).ToArray();
                }
            }
        }
    }
}
