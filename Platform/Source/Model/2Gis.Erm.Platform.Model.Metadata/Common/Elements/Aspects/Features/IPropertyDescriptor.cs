using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features
{
    public interface IPropertyDescriptor : IResourceDescriptor
    {
        string PropertyName { get; }
        Type Type { get; }
        bool TryGetValue(object container, out object result);
    }
}