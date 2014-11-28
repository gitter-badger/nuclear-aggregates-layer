using System;
using System.Globalization;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles
{
    public sealed class ResourceTitleDescriptor : ITitleDescriptor
    {
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
        
        public static ResourceTitleDescriptor Create<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            string keyName = StaticReflection.GetMemberName(resourceKeyExpression);
            Type resourceManagerType = StaticReflection.GetMemberDeclaringType(resourceKeyExpression);
            return new ResourceTitleDescriptor(new ResourceEntryKey(resourceManagerType, keyName));
        }

        public string GetValue(CultureInfo culture)
        {
            return ResourceEntryKey.ResourceHostType.AsResourceManager().GetObject(ResourceEntryKey.ResourceEntryName, culture).ToString();
        }

        public string ResourceKeyToString()
        {
            return ResourceEntryKey.ResourceEntryName;
        }

        public override string ToString()
        {
            return ResourceEntryKey.ResourceHostType.AsResourceManager().GetObject(ResourceEntryKey.ResourceEntryName).ToString();
        }
    }
}
