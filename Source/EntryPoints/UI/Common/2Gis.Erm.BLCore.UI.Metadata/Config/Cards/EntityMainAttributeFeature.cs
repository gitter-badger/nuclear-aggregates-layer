using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public class EntityMainAttributeFeature : IMetadataFeature
    {
        public EntityMainAttributeFeature(IPropertyDescriptor property)
        {
            Property = property;
        }

        public IPropertyDescriptor Property { get; private set; }
    }
}