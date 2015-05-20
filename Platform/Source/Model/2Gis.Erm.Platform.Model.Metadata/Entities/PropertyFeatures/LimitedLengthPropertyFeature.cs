using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class LimitedLengthPropertyFeature : IValidatablePropertyFeature
    {
        public LimitedLengthPropertyFeature(int minLenth, int maxLength)
        {
            MinLength = minLenth;
            MaxLength = maxLength;
        }

        public LimitedLengthPropertyFeature(int maxLength)
        {
            MinLength = 0;
            MaxLength = maxLength;
        }

        public int MinLength
        {
            get;
            private set;
        }

        public int MaxLength
        {
            get;
            private set;
        }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}
