using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities
{
    public interface IPropertyFeature : IConfigFeature
    {
        EntityProperty TargetProperty { get; set; }
    }
}
