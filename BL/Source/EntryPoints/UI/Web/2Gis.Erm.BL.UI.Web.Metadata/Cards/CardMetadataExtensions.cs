using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards
{
    public static class CardMetadataExtensions
    {
        public static CardStructure ToCardStructure(this CardMetadata card, CultureInfo culture)
        {
            var result = new CardStructure
                             {
                                 // Это странная штука
                                 DecimalDigits = 2
                             };

            if (card.ImageDescriptor != null)
            {
                result.Icon = card.ImageDescriptor.ToString();
                result.LargeIcon = card.ImageDescriptor.ToString();
            }

            result.EntityName = card.Entity.ToString();

            if (card.TitleDescriptor != null)
            {
                result.TitleResourceId = card.TitleDescriptor.ResourceKeyToString();
                result.Title = card.TitleDescriptor.GetValue(culture);
            }

            var mainAttributeFeature = card.Features<EntityMainAttributeFeature>().SingleOrDefault();
            if (mainAttributeFeature != null)
            {
                result.EntityMainAttribute = mainAttributeFeature.Property.PropertyName;
            }

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

            int SecurityPrivelegeFlag = 0;
            foreach (var feature in toolbarElement.Features<SecuredByFunctionalPrivelegeFeature>())
            {
                SecurityPrivelegeFlag |= (int)feature.Privilege;
            }

            foreach (var feature in toolbarElement.Features<SecuredByEntityPrivelegeFeature>())
            {
                SecurityPrivelegeFlag |= (int)feature.Privilege;
            }

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

            return result;
        }
    }
}
