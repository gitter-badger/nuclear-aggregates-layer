using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class MustBeGreaterOrEqualPropertyFeature : IValidatablePropertyFeature
    {
        public MustBeGreaterOrEqualPropertyFeature(string propertyName, Type resourceManagerType, string resourceKey)
        {
            PropertyToMatchName = propertyName;
            ErrorMessageResourceManagerType = resourceManagerType;
            ResourceKey = resourceKey;
        }

        /// <summary>
        /// Имя свойства, с которым идет сравнение
        /// </summary>
        public string PropertyToMatchName
        {
            get;
            private set;
        }

        public Type ErrorMessageResourceManagerType { get; private set; }

        public string ResourceKey { get; private set; }

        public static MustBeGreaterOrEqualPropertyFeature Create<TKey>(Expression<Func<TKey, object>> displayPropertyExpression, Expression<Func<string>> errorMessageResourceKeyExpression)
        {
            var propertyName = StaticReflection.GetMemberName(displayPropertyExpression);
            var errorMessageKeyName = StaticReflection.GetMemberName(errorMessageResourceKeyExpression);
            var errorMessageResourceManagerType = StaticReflection.GetMemberDeclaringType(errorMessageResourceKeyExpression);
            return new MustBeGreaterOrEqualPropertyFeature(propertyName, errorMessageResourceManagerType, errorMessageKeyName);
        }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
    }
}
