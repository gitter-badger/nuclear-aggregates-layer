using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class ExcludeZeroValuePropertyFeature : IPropertyFeature
    {
        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}
