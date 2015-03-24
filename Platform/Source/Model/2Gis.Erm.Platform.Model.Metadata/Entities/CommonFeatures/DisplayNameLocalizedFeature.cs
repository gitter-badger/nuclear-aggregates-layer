using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

using NuClear.Metamodeling.Domain.Entities;
using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures
{
    public sealed class DisplayNameLocalizedFeature : IPropertyFeature, IDataFieldFeature, IUniqueMetadataFeature
    {
        public DisplayNameLocalizedFeature(Type resourceManagerType, string resourceKey)
        {
            ResourceManagerType = resourceManagerType;
            ResourceKey = resourceKey;
        }

        public Type ResourceManagerType { get; private set; }

        public string ResourceKey { get; private set; }
        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }
        public static DisplayNameLocalizedFeature Create<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            var keyName = StaticReflection.GetMemberName(resourceKeyExpression);
            var resourceManagerType = StaticReflection.GetMemberDeclaringType(resourceKeyExpression);
            return new DisplayNameLocalizedFeature(resourceManagerType, keyName);
        }
    }
}
