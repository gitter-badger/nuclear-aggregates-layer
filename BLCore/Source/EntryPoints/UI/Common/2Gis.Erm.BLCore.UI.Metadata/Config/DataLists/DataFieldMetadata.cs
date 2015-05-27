using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataFieldMetadata : MetadataElement<DataFieldMetadata, DataFieldMetadataBuilder>, IMetadataElementAspect
    {
        private readonly IMetadataElementIdentity _identity;

        public DataFieldMetadata(IEnumerable<IMetadataFeature> features) : base(features)
        {
            _identity = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.Stub().Unique().Build().AsIdentity();
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public Type Type
        {
            get { return this.Feature<DataFieldExpressionFeature>().PropertyType; }
        }

        public string Expression
        {
            get { return this.Feature<DataFieldExpressionFeature>().Expression; }
        }

        public string PropertyName
        {
            get { return this.Feature<DataFieldExpressionFeature>().PropertyName; }
        }

        public string NameLocalizedResourceKey
        {
            get
            {
                var feature = this.Features<DisplayNameLocalizedFeature>().SingleOrDefault();
                return feature != null ? feature.ResourceKey : null;
            }
        }

        public Type NameLocalizedResourceManagerType
        {
            get
            {
                var feature = this.Features<DisplayNameLocalizedFeature>().SingleOrDefault();
                return feature != null ? feature.ResourceManagerType : null;
            }
        }

        public bool IsMainAttribute
        {
            get { return this.Features<MainAttributeFeature>().SingleOrDefault() != null; }
        }

        public bool IsHidden
        {
            get { return this.Features<HiddenFeature>().SingleOrDefault() != null; }
        }

        public bool IsFiltered
        {
            get { return this.Features<FilteredDataFieldFeature>().SingleOrDefault() != null; }
        }

        public bool IsSortingEnabled
        {
            get { return this.Features<DisableSortingDataFieldFeature>().SingleOrDefault() == null; }
        }

        public bool IsReference
        {
            get { return this.Features<ReferenceDataFieldFeature>().SingleOrDefault() != null; }
        }

        // FIXME {a.tukaev}: Перевести ListDto на поля типа EntityReference 
        public string ReferencedPropertyName
        {
            get
            {
                var feature = this.Features<ReferenceDataFieldFeature>().SingleOrDefault();
                return feature != null ? feature.ReferencedPropertyName : null;
            }
        }

        public IEntityType ReferencedEntityName
        {
            get
            {
                var feature = this.Features<ReferenceDataFieldFeature>().SingleOrDefault();
                return feature != null ? feature.ReferencedEntityName : EntityType.Instance.None();
            }
        }

        // TODO {a.tukaev, 09.08.2013}: Попробовать заиспользовать что-то типа ComposedDataFieldFeature -> будет два филда, Name и Id...
        public IEnumerable<DataFieldMetadata> ExtraDataFields
        {
            get
            {
                var feature = this.Features<ExtraDataFieldsFeature>().SingleOrDefault();
                return feature != null ? feature.ExtraDataFields : Enumerable.Empty<DataFieldMetadata>();
            }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotImplementedException();
        }
    }
}