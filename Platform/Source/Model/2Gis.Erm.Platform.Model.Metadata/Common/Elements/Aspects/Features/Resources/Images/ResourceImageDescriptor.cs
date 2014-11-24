using System;
using System.Globalization;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images
{
    public sealed class ResourceImageDescriptor : IImageDescriptor
    {
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
        
        public static ResourceImageDescriptor Create<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            string keyName = StaticReflection.GetMemberName(resourceKeyExpression);
            Type resourceManagerType = StaticReflection.GetMemberDeclaringType(resourceKeyExpression);
            return new ResourceImageDescriptor(new ResourceEntryKey(resourceManagerType, keyName));
        }

        public object GetValue(CultureInfo culture)
        {
            return ResourceEntryKey.ResourceHostType.AsResourceManager().GetObject(ResourceEntryKey.ResourceEntryName, culture);
        }

        public string ResourceKeyToString()
        {
            return ResourceEntryKey.ResourceEntryName;
        }
    }
}