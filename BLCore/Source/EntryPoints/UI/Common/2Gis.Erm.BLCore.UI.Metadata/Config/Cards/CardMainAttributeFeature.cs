using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public sealed class CardMainAttributeFeature : ICardFeature
    {
        public CardMainAttributeFeature(IPropertyDescriptor propertyDescriptor)
        {
            PropertyDescriptor = propertyDescriptor;
        }

        public IPropertyDescriptor PropertyDescriptor { get; private set; }
        public string PropertyName
        {
            get { return PropertyDescriptor.PropertyName; }
        }
    }
}