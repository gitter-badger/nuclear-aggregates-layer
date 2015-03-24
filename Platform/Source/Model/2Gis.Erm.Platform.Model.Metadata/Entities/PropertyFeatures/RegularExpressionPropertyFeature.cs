using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class RegularExpressionPropertyFeature : IValidatablePropertyFeature
    {
        public RegularExpressionPropertyFeature(string regularExpression, Type resourceManagerType, string resourceKey)
        {
            RegularExpression = regularExpression;
            ErrorMessageResourceManagerType = resourceManagerType;
            ResourceKey = resourceKey;
        }

        public string RegularExpression
        {
            get; 
            private set; 
        }

        public Type ErrorMessageResourceManagerType { get; private set; }

        public string ResourceKey { get; private set; }

        public static RegularExpressionPropertyFeature Create<TKey>(string regularExpression, Expression<Func<TKey>> errorMessageResourceKeyExpression)
        {
            var errorMessageKeyName = StaticReflection.GetMemberName(errorMessageResourceKeyExpression);
            var errorMessageResourceManagerType = StaticReflection.GetMemberDeclaringType(errorMessageResourceKeyExpression);
            return new RegularExpressionPropertyFeature(regularExpression, errorMessageResourceManagerType, errorMessageKeyName);
        }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}
