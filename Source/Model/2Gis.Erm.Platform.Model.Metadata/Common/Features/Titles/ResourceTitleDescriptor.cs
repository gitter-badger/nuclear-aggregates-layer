using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles
{
    public class ResourceTitleDescriptor : ITitleDescriptor
    {
        public static ResourceTitleDescriptor Create<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            string keyName = StaticReflection.GetMemberName(resourceKeyExpression);
            Type resourceManagerType = StaticReflection.GetMemberDeclaringType(resourceKeyExpression);
            return new ResourceTitleDescriptor(new ResourceEntryKey(resourceManagerType, keyName));
        }

        private readonly ResourceEntryKey _resourceEntryKey;

        public ResourceTitleDescriptor(ResourceEntryKey resourceEntryKey)
        {
            _resourceEntryKey = resourceEntryKey;
        }

        public ResourceEntryKey ResourceEntryKey
        {
            get
            {
                return _resourceEntryKey;
            }
        }
    }
}
