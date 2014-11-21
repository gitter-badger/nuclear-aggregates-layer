using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Entities
{
    public interface IPropertyDescriptor 
    {
        string PropertyName { get; }
        EntityName Entity { get; }
    }
}