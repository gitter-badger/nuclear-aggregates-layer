using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities.Concrete;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities
{
    public sealed class EntityPropertyMetadata : MetadataElement
    {
        private IMetadataElementIdentity _identity;

        public EntityPropertyMetadata(string name, Type propertyType)
            : base(Enumerable.Empty<IMetadataFeature>())
        {
            _identity = new MetadataElementIdentity(new Uri(name, UriKind.Relative));
            Name = name;
            Type = propertyType;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public Type DeclaringType { get; set; }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public static EntityPropertyMetadata Create<TKey>(Expression<Func<TKey, object>> propertyExpression)
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            var propertyType = StaticReflection.GetMemberType(propertyExpression);
            var declaringType = StaticReflection.GetMemberDeclaringType(propertyExpression);
            return new EntityPropertyMetadata(propertyName, propertyType) { DeclaringType = declaringType };
        }

        public EntityPropertyMetadata WithFeatures(params IPropertyFeature[] features)
        {
            var updater = (IMetadataElementUpdater)this;
            foreach (var feature in features)
            {
                updater.AddFeature(feature);
            }

            return this;
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

        public override string ToString()
        {
            return DeclaringType != null ? DeclaringType.Name + ". " : string.Empty + string.Format("{0}. {1}", Type.Name, Name);
        }
    }
}
