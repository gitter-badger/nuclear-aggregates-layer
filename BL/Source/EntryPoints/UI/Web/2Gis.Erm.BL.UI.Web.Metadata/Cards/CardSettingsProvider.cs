using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Handler.Concrete;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards
{
    public sealed class CardSettingsProvider : ICardSettingsProvider
    {
        private readonly IGlobalizationSettings _globalizationSettings;

        private readonly IMetadataProvider _metadataProvider;
        private CultureInfo _currentCulture;

        public CardSettingsProvider(IGlobalizationSettings globalizationSettings, IMetadataProvider metadataProvider)
        {
            _globalizationSettings = globalizationSettings;
            _metadataProvider = metadataProvider;
        }

        // TODO {all, 24.12.2014}: Культура передается т.к. на старте приложения проверяется корректность метаданных.
        // Контекст пользователя в это время неопределен. После удаления метаданных карточек из EntitySettings.xml и соответсвующей проверки метаданных карточек
        // культуру можно будет брать из контекста пользователя
        public CardStructure GetCardSettings<TEntity>(CultureInfo culture) where TEntity : IEntity
        {
            _currentCulture = culture;
            return GetCardSettings(typeof(TEntity).AsEntityName(), culture);
        }

        // TODO {all, 24.12.2014}: Культура передается т.к. на старте приложения проверяется корректность метаданных.
        // Контекст пользователя в это время неопределен. После удаления метаданных карточек из EntitySettings.xml и соответсвующей проверки метаданных карточек
        // культуру можно будет брать из контекста пользователя
        public CardStructure GetCardSettings(IEntityType entity, CultureInfo culture)
        {
            _currentCulture = culture;
            CardMetadata metadata;
            if (!_metadataProvider.TryGetMetadata(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(entity.Description).Build().AsIdentity().Id, out metadata))
            {
                throw new ArgumentException(string.Format("Cannot find metadata for entity {0}", entity), "entity");
            }

            return ToCardStructure(metadata);
        }

        private CardStructure ToCardStructure(CardMetadata card)
        {            
            var result = new CardStructure
            {
                DecimalDigits = _globalizationSettings.SignificantDigitsNumber
            };

            if (card.ImageDescriptor != null)
            {
                result.Icon = card.ImageDescriptor.ToString();
            }

            result.EntityName = card.Entity.Description;

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

            var mainAttributeFeature = card.Features<MainAttributeFeature>().SingleOrDefault();
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

        private IEnumerable<ToolbarElementStructure> ToToolbarStructure(UIElementMetadata toolbarElement, UIElementMetadata parentElement)
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

        private CardRelatedItemsGroupStructure ToCardRelatedItemsGroupStructure(RelatedItemsFeature relatedItemsElement)
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

        private CardRelatedItemStructure ToCardRelatedItemStructure(UIElementMetadata element)
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
                result.AppendableEntity = appendableEntityFeature.Entity.Description;
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
