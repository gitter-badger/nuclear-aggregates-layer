using System.Collections.Generic;
using System.Globalization;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources
{
    public interface ITemplateDescriptor : IResourceDescriptor
    {
        IResourceDescriptor Template { get; }
        IEnumerable<IResourceDescriptor> TemplateParameters { get; }
        bool TryFormat(CultureInfo culture, out string result, params object[] templateParameterValueContainers);
    }
}
