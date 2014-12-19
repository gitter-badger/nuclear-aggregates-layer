using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler.Concrete;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards
{
    public sealed class CardSettingsProvider : ICardSettingsProvider
    {
        private readonly IGlobalizationSettings _globalizationSettings;

        private readonly IMetadataProvider _metadataProvider;

        private readonly CultureInfo _currentCulture;

        public CardSettingsProvider(IGlobalizationSettings globalizationSettings, IMetadataProvider metadataProvider, IUserContext userContext)
        {
            _globalizationSettings = globalizationSettings;
            _metadataProvider = metadataProvider;
            _currentCulture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
        }

        public CardStructure GetCardSettings<TEntity>() where TEntity : IEntity
        {
            return GetCardSettings(typeof(TEntity).AsEntityName());
        }

        public CardStructure GetCardSettings(EntityName entity)
        {
            CardMetadata metadata;
            if (!_metadataProvider.TryGetMetadata(IdBuilder.For<MetadataCardsIdentity>(entity.ToString()).AsIdentity().Id, out metadata))
            {
                throw new ArgumentException(string.Format("Cannot find metadata for entity {0}", entity), "entity");
            }

            return ToCardStructure(metadata);
        }

        internal CardStructure ToCardStructure(CardMetadata card)
        {            
            var result = new CardStructure
            {
                DecimalDigits = _globalizationSettings.SignificantDigitsNumber
            };

            if (card.ImageDescriptor != null)
            {
                result.Icon = card.ImageDescriptor.ToString();
            }

            result.EntityName = card.Entity.ToString();

            if (card.TitleDescriptor != null)
            {
                result.TitleResourceId = card.TitleDescriptor.ResourceKeyToString();
                result.Title = card.TitleDescriptor.GetValue(_currentCulture);
            }

            if (card.EntityLocalizationDescriptor != null)
            {
                result.EntityNameLocaleResourceId = card.EntityLocalizationDescriptor.ResourceKeyToString();
                result.EntityLocalizedName = card.EntityLocalizationDescriptor.GetValue(_currentCulture);
            }

            var mainAttributeFeature = card.Features<ICardMainAttributeFeature>().SingleOrDefault();
            if (mainAttributeFeature != null)
            {
                result.EntityMainAttribute = mainAttributeFeature.PropertyDescriptor.PropertyName;
            }

            result.HasComments = card.Uses<ShowNotesFeature>();
            result.HasAdminTab = card.Uses<ShowAdminPartFeature>();

            result.CardToolbar = card.ActionsDescriptors.SelectMany(x => ToToolbarStructure(x, null)).ToArray();
            result.CardRelatedItems = card.Features<RelatedItemsFeature>().Select(ToCardRelatedItemsGroupStructure).ToArray();

            return result;
        }

        internal IEnumerable<ToolbarElementStructure> ToToolbarStructure(UIElementMetadata toolbarElement, UIElementMetadata parentElement)
        {
            var resultElement = new ToolbarElementStructure();
            var result = new List<ToolbarElementStructure>
                             {
                                 resultElement
                             };

            if (toolbarElement.NameDescriptor != null)
            {
                resultElement.Name = toolbarElement.NameDescriptor.ToString();
            }

            var controlType = toolbarElement.Features<ControlTypeFeature>().SingleOrDefault();
            if (controlType != null)
            {
                resultElement.ControlType = controlType.ControlTypeDescriptor.ToString();
            }

            if (toolbarElement.HasHandler)
            {
                resultElement.Action = toolbarElement.Handler.ToString();
            }

            if (toolbarElement.ImageDescriptor != null)
            {
                resultElement.Icon = toolbarElement.ImageDescriptor.ToString();
            }

            if (toolbarElement.TitleDescriptor != null)
            {
                resultElement.NameLocaleResourceId = toolbarElement.TitleDescriptor.ResourceKeyToString();
                resultElement.LocalizedName = toolbarElement.TitleDescriptor.GetValue(_currentCulture);
            }

            resultElement.LockOnNew = toolbarElement.Uses<LockOnNewCardFeature>();
            resultElement.LockOnInactive = toolbarElement.Uses<LockOnInactiveCardFeature>();

            var securityPrivelegeFlag = 0;
            foreach (var feature in toolbarElement.Features<SecuredByFunctionalPrivelegeFeature>())
            {
                securityPrivelegeFlag |= (int)feature.Privilege;
            }

            foreach (var feature in toolbarElement.Features<SecuredByEntityPrivelegeFeature>())
            {
                securityPrivelegeFlag |= (int)feature.Privilege;
            }

            resultElement.SecurityPrivelege = securityPrivelegeFlag;

            if (parentElement != null)
            {
                resultElement.ParentName = parentElement.NameDescriptor.ToString();
            }

            result.AddRange(toolbarElement.Elements.OfType<UIElementMetadata>().SelectMany(x => ToToolbarStructure(x, toolbarElement)));

            return result;
        }

        internal CardRelatedItemsGroupStructure ToCardRelatedItemsGroupStructure(RelatedItemsFeature relatedItemsElement)
        {
            var result = new CardRelatedItemsGroupStructure();
            if (relatedItemsElement.NameDescriptor != null)
            {
                result.Name = relatedItemsElement.NameDescriptor.ToString();
            }

            if (relatedItemsElement.TitleDescriptor != null)
            {
                result.NameLocaleResourceId = relatedItemsElement.TitleDescriptor.ResourceKeyToString();
                result.LocalizedName = relatedItemsElement.TitleDescriptor.GetValue(_currentCulture);
            }

            result.Items = relatedItemsElement.RelatedItems.Select(ToCardRelatedItemStructure).ToArray();

            return result;
        }

        internal CardRelatedItemStructure ToCardRelatedItemStructure(UIElementMetadata element)
        {
            var result = new CardRelatedItemStructure();
            if (element.NameDescriptor != null)
            {
                result.Name = element.NameDescriptor.GetValue(_currentCulture);
            }

            if (element.HasHandler)
            {
                var showGridHandler = element.Handler as ShowGridHandlerFeature;
                if (showGridHandler != null)
                {
                    result.RequestUrl = showGridHandler.ToRequestUrl();
                }

                var requestHandler = element.Handler as RequestHandlerFeature;
                if (requestHandler != null)
                {
                    result.RequestUrl = requestHandler.RequestDescriptor.ToString();
                }
            }

            if (element.ImageDescriptor != null)
            {
                result.Icon = element.ImageDescriptor.ResourceKeyToString();
            }

            if (element.TitleDescriptor != null)
            {
                result.NameLocaleResourceId = element.TitleDescriptor.ResourceKeyToString();
                result.LocalizedName = element.TitleDescriptor.GetValue(_currentCulture);
            }

            var lockOnNewFeature = element.Features<LockOnNewCardFeature>().SingleOrDefault();
            if (lockOnNewFeature != null)
            {
                result.AppendDisabledExpression(lockOnNewFeature.ToDisabledExpression());
            }

            var appendableEntityFeature = element.Features<AppendableEntityFeature>().SingleOrDefault();
            if (appendableEntityFeature != null)
            {
                result.AppendableEntity = appendableEntityFeature.Entity.ToString();
            }

            var filterToParentFeature = element.Features<FilterToParentFeature>().SingleOrDefault();
            if (filterToParentFeature != null)
            {
                result.AppendExtendedInfo(filterToParentFeature.ToExtendedInfo());
            }

            var filterToParentsFeature = element.Features<FilterByParentsFeature>().SingleOrDefault();
            if (filterToParentsFeature != null)
            {
                result.AppendExtendedInfo(filterToParentsFeature.ToExtendedInfo());
            }

            return result;
        }
    }
}
