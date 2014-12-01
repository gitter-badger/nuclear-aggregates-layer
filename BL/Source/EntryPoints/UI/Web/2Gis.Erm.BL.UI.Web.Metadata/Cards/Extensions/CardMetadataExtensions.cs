using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler.Concrete;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class CardMetadataExtensions
    {
        public static CardStructure ToCardStructure(this CardMetadata card, CultureInfo culture, IBusinessModelSettings businessModelSettings)
        {
            var result = new CardStructure
                             {
                                 DecimalDigits = businessModelSettings.SignificantDigitsNumber
                             };

            if (card.ImageDescriptor != null)
            {
                result.Icon = card.ImageDescriptor.ResourceKeyToString(); 
            }

            result.EntityName = card.Entity.ToString();

            if (card.TitleDescriptor != null)
            {
                result.TitleResourceId = card.TitleDescriptor.ResourceKeyToString();
                result.Title = card.TitleDescriptor.GetValue(culture);
            }

            if (card.EntityLocalizationDescriptor != null)
            {
                result.EntityNameLocaleResourceId = card.EntityLocalizationDescriptor.ResourceKeyToString();
                result.EntityLocalizedName = card.EntityLocalizationDescriptor.GetValue(culture);
            }

            var mainAttributeFeature = card.Features<CardMainAttributeFeature>().SingleOrDefault();
            if (mainAttributeFeature != null)
            {
                result.EntityMainAttribute = mainAttributeFeature.Property.PropertyName;
            }

            result.CardToolbar = card.ActionsDescriptors.Select(x => x.ToToolbarStructure(culture)).ToArray();
            result.CardRelatedItems = card.Features<RelatedItemsFeature>().Select(x => x.ToCardRelatedItemsGroupStructure(culture)).ToArray();

            return result;
        }

        public static ToolbarElementStructure ToToolbarStructure(this UiElementMetadata toolbarElement, CultureInfo culture)
        {
            var result = new ToolbarElementStructure();
            if (toolbarElement.NameDescriptor != null)
            {
                result.Name = toolbarElement.NameDescriptor.GetValue(culture);
            }

            var controlType = toolbarElement.Features<ControlTypeFeature>().SingleOrDefault();
            if (controlType != null)
            {
                result.ControlType = controlType.ControlTypeDescriptor.ToString();
            }

            if (toolbarElement.HasHandler)
            {
                result.Action = toolbarElement.Handler.ToString();
            }

            if (toolbarElement.ImageDescriptor != null)
            {
                result.Icon = toolbarElement.ImageDescriptor.ToString();
            }

            if (toolbarElement.TitleDescriptor != null)
            {
                result.NameLocaleResourceId = toolbarElement.TitleDescriptor.ResourceKeyToString();
                result.LocalizedName = toolbarElement.TitleDescriptor.GetValue(culture);
            }

            result.LockOnNew = toolbarElement.Uses<LockOnNewFeature>();
            result.LockOnInactive = toolbarElement.Uses<LockOnInactiveFeature>();

            int securityPrivelegeFlag = 0;
            foreach (var feature in toolbarElement.Features<SecuredByFunctionalPrivelegeFeature>())
            {
                securityPrivelegeFlag |= (int)feature.Privilege;
            }

            foreach (var feature in toolbarElement.Features<SecuredByEntityPrivelegeFeature>())
            {
                securityPrivelegeFlag |= (int)feature.Privilege;
            }

            result.SecurityPrivelege = securityPrivelegeFlag;

            return result;
        }

        public static CardRelatedItemsGroupStructure ToCardRelatedItemsGroupStructure(this RelatedItemsFeature relatedItemsElement, CultureInfo culture)
        {
            var result = new CardRelatedItemsGroupStructure();
            if (relatedItemsElement.NameDescriptor != null)
            {
                result.Name = relatedItemsElement.NameDescriptor.ToString();
            }

            if (relatedItemsElement.TitleDescriptor != null)
            {
                result.NameLocaleResourceId = relatedItemsElement.TitleDescriptor.ResourceKeyToString();
                result.LocalizedName = relatedItemsElement.TitleDescriptor.GetValue(culture);
            }

            result.Items = relatedItemsElement.RelatedItems.Select(x => x.ToCardRelatedItemStructure(culture));

            return result;
        }

        public static CardRelatedItemStructure ToCardRelatedItemStructure(this UiElementMetadata element, CultureInfo culture)
        {
            var result = new CardRelatedItemStructure();
            if (element.NameDescriptor != null)
            {
                result.Name = element.NameDescriptor.GetValue(culture);
            }

            if (element.HasHandler)
            {
                var showGridHandler = element.Handler as ShowGridHandlerFeature;
                if (showGridHandler != null)
                {
                    result.RequestUrl = showGridHandler.ToRequestUrl();
                }
            }

            if (element.ImageDescriptor != null)
            {
                result.Icon = element.ImageDescriptor.ResourceKeyToString();
            }

            if (element.TitleDescriptor != null)
            {
                result.NameLocaleResourceId = element.TitleDescriptor.ResourceKeyToString();
                result.LocalizedName = element.TitleDescriptor.GetValue(culture);
            }

            var lockOnNewFeature = element.Features<LockOnNewFeature>().SingleOrDefault();
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

            return result;
        }
    }
}