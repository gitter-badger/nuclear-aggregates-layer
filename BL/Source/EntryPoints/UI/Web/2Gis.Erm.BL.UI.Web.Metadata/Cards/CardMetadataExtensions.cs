using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

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

            return result;
        }
    }
}
