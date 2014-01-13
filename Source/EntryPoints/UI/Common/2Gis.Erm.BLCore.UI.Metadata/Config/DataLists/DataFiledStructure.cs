using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataFieldStructure : ConfigElement<DataFieldStructure, OrdinaryConfigElementIdentity, DataFieldStructureBuilder>, IConfigElementAspect
    {
        private readonly OrdinaryConfigElementIdentity _identity;
        private readonly Lazy<DataFieldExpressionFeature> _dataFieldExpressionFeature;
        private readonly Lazy<DisplayNameLocalizedFeature> _displayNameLocalizedFeature;
        private readonly Lazy<MainAttributeFeature> _mainAttributeFeature;
        private readonly Lazy<HiddenFeature> _hiddenFeature;
        private readonly Lazy<FilteredDataFieldFeature> _filteredDataFieldFeature;
        private readonly Lazy<DisableSortingDataFieldFeature> _disableSortingDataFieldFeature;
        private readonly Lazy<ReferenceDataFieldFeature> _referenceDataFieldFeature;
        private readonly Lazy<ExtraDataFieldsFeature> _extraDataFieldsFeature; 
        public DataFieldStructure(IEnumerable<IConfigFeature> features) : base(features)
        {
            _identity = new OrdinaryConfigElementIdentity();
            _dataFieldExpressionFeature = new Lazy<DataFieldExpressionFeature>(() => ElementFeatures.OfType<DataFieldExpressionFeature>().Single());
            _displayNameLocalizedFeature = new Lazy<DisplayNameLocalizedFeature>(() => ElementFeatures.OfType<DisplayNameLocalizedFeature>().SingleOrDefault());
            _mainAttributeFeature = new Lazy<MainAttributeFeature>(() => ElementFeatures.OfType<MainAttributeFeature>().SingleOrDefault());
            _hiddenFeature = new Lazy<HiddenFeature>(() => ElementFeatures.OfType<HiddenFeature>().SingleOrDefault());
            _filteredDataFieldFeature = new Lazy<FilteredDataFieldFeature>(() => ElementFeatures.OfType<FilteredDataFieldFeature>().SingleOrDefault());
            _disableSortingDataFieldFeature =
                new Lazy<DisableSortingDataFieldFeature>(() => ElementFeatures.OfType<DisableSortingDataFieldFeature>().SingleOrDefault());
            _referenceDataFieldFeature = new Lazy<ReferenceDataFieldFeature>(() => ElementFeatures.OfType<ReferenceDataFieldFeature>().SingleOrDefault());
            _extraDataFieldsFeature = new Lazy<ExtraDataFieldsFeature>(() => ElementFeatures.OfType<ExtraDataFieldsFeature>().SingleOrDefault());
        }

        public override IConfigElementIdentity ElementIdentity
        {
            get { return _identity; }
        }

        public override OrdinaryConfigElementIdentity Identity
        {
            get { return _identity; }
        }

        public Type Type
        {
            get
            {
                return _dataFieldExpressionFeature.Value.PropertyType;
            }
        }

        public string Expression
        {
            get { return _dataFieldExpressionFeature.Value.Expression; }
        }

        public string PropertyName
        {
            get { return _dataFieldExpressionFeature.Value.PropertyName; }
        }

        public string NameLocalizedResourceKey
        {
            get { return _displayNameLocalizedFeature.Value != null ? _displayNameLocalizedFeature.Value.ResourceKey : null; }
        }

        public Type NameLocalizedResourceManagerType
        {
            get { return _displayNameLocalizedFeature.Value != null ? _displayNameLocalizedFeature.Value.ResourceManagerType : null; }
        }

        public bool IsMainAttribute
        {
            get { return _mainAttributeFeature.Value != null; }
        }

        public bool IsHidden
        {
            get { return _hiddenFeature.Value != null; }
        }

        public bool IsFiltered
        {
            get { return _filteredDataFieldFeature.Value != null; }
        }

        public bool IsSortingEnabled
        {
            get { return _disableSortingDataFieldFeature.Value == null; }
        }

        public bool IsReference
        {
            get { return _referenceDataFieldFeature.Value != null; }
        }

        // FIXME {a.tukaev}: Перевести ListDto на поля типа EntityReference 
        public string ReferencedPropertyName
        {
            get { return _referenceDataFieldFeature.Value != null ? _referenceDataFieldFeature.Value.ReferencedPropertyName : null; }
        }

        public EntityName ReferencedEntityName
        {
            get { return _referenceDataFieldFeature.Value != null ? _referenceDataFieldFeature.Value.ReferencedEntityName : EntityName.None; }
        }

        // TODO {a.tukaev, 09.08.2013}: Попробовать заиспользовать что-то типа ComposedDataFieldFeature -> будет два филда, Name и Id...
        public IEnumerable<DataFieldStructure> ExtraDataFields
        {
            get { return _extraDataFieldsFeature.Value != null ? _extraDataFieldsFeature.Value.ExtraDataFields : Enumerable.Empty<DataFieldStructure>(); }
        } 
    }
}