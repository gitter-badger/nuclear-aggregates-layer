using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public class GroupedPropertyFeature : IPropertyFeature
    {
        public GroupedPropertyFeature(ResourceEntryKey resourceEntryKey)
        {
            EntryKey = resourceEntryKey;
        }

        public ResourceEntryKey EntryKey { get; private set; }

        public static GroupedPropertyFeature Create<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            return new GroupedPropertyFeature(ResourceEntryKey.Create(resourceKeyExpression));
        }

        public EntityProperty TargetProperty { get; set; }
    }
}