using System.Globalization;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources
{
    public interface IStringResourceDescriptor : IResourceDescriptor
    {
        string GetValue(CultureInfo culture);
    }
}