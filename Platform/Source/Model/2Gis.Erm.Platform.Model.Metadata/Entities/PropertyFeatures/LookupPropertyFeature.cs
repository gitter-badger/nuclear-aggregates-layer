using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.Properties.Configuration;

using NuClear.Metamodeling.Domain.Entities;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures
{
    public sealed class LookupPropertyFeature : IPropertyFeature
    {
        public LookupPropertyFeature(IEntityType lookupEntityType)
        {
            LookupEntity = lookupEntityType;

            // by default
            KeyAttribute = LookupAttributeProvider.GetDefaultKeyAttribute(lookupEntityType);
            ValueAttribute = LookupAttributeProvider.GetDefaultValueAttribute(lookupEntityType);
        }

        public IEntityType LookupEntity
        {
            get; 
            private set;
        }

        public EntityPropertyMetadata TargetPropertyMetadata { get; set; }

        public bool ShowReadOnlyCard { get; set; }
        public bool Disabled { get; set; }
        public bool ReadOnly { get; set; }
        public string ExtendedInfo { get; set; }

        public string KeyAttribute { get; set; }
        public string ValueAttribute { get; set; }

        public static LookupPropertyFeature Create(IEntityType lookupEntityType)
        {
            return new LookupPropertyFeature(lookupEntityType);
        }

        public LookupPropertyFeature WithShowReadOnlyCard()
        {
            ShowReadOnlyCard = true;
            return this;
        }

        public LookupPropertyFeature WithDisabled()
        {
            Disabled = true;
            return this;
        }

        public LookupPropertyFeature WithExtendedInfo(string extendedInfo)
        {
            ExtendedInfo = extendedInfo;
            return this;
        }

        public LookupPropertyFeature WithReadOnly()
        {
            ReadOnly = true;
            return this;
        }

        public LookupPropertyFeature OverrideKeyAttribute(string propertyName)
        {
            KeyAttribute = propertyName;
            return this;
        }

        public LookupPropertyFeature OverrideValueAttribute(string propertyName)
        {
            ValueAttribute = propertyName;
            return this;
        }

        public LookupPropertyFeature OverrideKeyAttribute<TDto>(Expression<Func<TDto, long>> keyExpression)
        {
            KeyAttribute = StaticReflection.GetMemberName(keyExpression);
            return this;
        }

        public LookupPropertyFeature OverrideValueAttribute<TDto>(Expression<Func<TDto, string>> valueExpression)
        {
            ValueAttribute = StaticReflection.GetMemberName(valueExpression);
            return this;
        }
    }
}
