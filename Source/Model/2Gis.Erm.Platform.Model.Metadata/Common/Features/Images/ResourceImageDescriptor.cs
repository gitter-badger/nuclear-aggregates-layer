using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Images
{
    public class ResourceImageDescriptor : IImageDescriptor
    {
        public static ResourceImageDescriptor Create<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            string keyName = StaticReflection.GetMemberName(resourceKeyExpression);
            Type resourceManagerType = StaticReflection.GetMemberDeclaringType(resourceKeyExpression);
            return new ResourceImageDescriptor(new ResourceEntryKey(resourceManagerType, keyName));
        }

        private readonly ResourceEntryKey _resourceEntryKey;

        public ResourceImageDescriptor(ResourceEntryKey resourceEntryKey)
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