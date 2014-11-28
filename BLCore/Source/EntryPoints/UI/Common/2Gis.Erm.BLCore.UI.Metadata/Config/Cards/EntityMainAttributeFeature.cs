using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{

    public class CardMainAttributeFeature : IMetadataFeature
    {
        public CardMainAttributeFeature(IPropertyDescriptor property)
        {
            Property = property;
        }

        // TODO {y.baranihin, 28.11.2014}: Заменить на стандартный PropertyDescriptor
        public IPropertyDescriptor Property { get; private set; }
    }
}