using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public class CardMainAttributeFeature : ICardMainAttributeFeature
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