using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public sealed class MainAttributeFeature : IUniqueMetadataFeature
    {
        public MainAttributeFeature(IPropertyDescriptor propertyDescriptor)
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