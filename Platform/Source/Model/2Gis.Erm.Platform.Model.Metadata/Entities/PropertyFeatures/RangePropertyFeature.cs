using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class RangePropertyFeature : IValidatablePropertyFeature
    {
        public RangePropertyFeature(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public RangePropertyFeature(int minValue, int maxValue, Type errorMessageResourceManagerType, string errorMessageKeyName)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            ErrorMessageResourceManagerType = errorMessageResourceManagerType;
            ResourceKey = errorMessageKeyName;
        }

        public RangePropertyFeature(int maxValue)
        {
            MinValue = 0;
            MaxValue = maxValue;
        }

        public int MinValue
        {
            get;
            private set;
        }

        public int MaxValue
        {
            get;
            private set;
        }

        public Type ErrorMessageResourceManagerType
        {
            get; 
            private set;
        }

        public string ResourceKey 
        { 
            get; 
            private set; 
        }

        public static RangePropertyFeature Create<TKey>(int minValue, int maxValue, Expression<Func<TKey>> errorMessageResourceKeyExpression)
        {
            var errorMessageKeyName = StaticReflection.GetMemberName(errorMessageResourceKeyExpression);
            var errorMessageResourceManagerType = StaticReflection.GetMemberDeclaringType(errorMessageResourceKeyExpression);
            return new RangePropertyFeature(minValue, maxValue, errorMessageResourceManagerType, errorMessageKeyName);
        }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}      