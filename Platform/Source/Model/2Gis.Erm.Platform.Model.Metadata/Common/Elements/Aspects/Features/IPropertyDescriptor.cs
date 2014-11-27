using System;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features
{
    public interface IPropertyDescriptor 
    {
        string PropertyName { get; }
        Type Type { get; }
    }
}