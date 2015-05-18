using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class MultilinePropertyFeature : IPropertyFeature
    {
        public MultilinePropertyFeature()
        {
        }

        public MultilinePropertyFeature(int recommendedLinesAmount)
        {
            RecommendedLinesAmount = recommendedLinesAmount;
        }

        public int? RecommendedLinesAmount
        {
            get;
            private set;
        }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}
