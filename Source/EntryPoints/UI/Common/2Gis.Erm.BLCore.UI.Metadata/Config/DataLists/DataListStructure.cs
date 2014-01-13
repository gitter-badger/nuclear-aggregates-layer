using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataListStructure : ConfigElement<DataListStructure, DataListStructureIdentity, DataListStructureBuilder>, IOperationsBoundElement
    {
        private readonly DataListStructureIdentity _identity;
        private readonly Lazy<DisplayNameLocalizedFeature> _displayNameLocalizedFeature;
        private readonly Lazy<DefaultFilterFeature> _defaultFilterFeature;
        private readonly Lazy<SortDescendingFeature> _sortDescendingFeature;
        private readonly Lazy<DataFieldsFeature> _dataFieldsFeature;
        private readonly Lazy<BatchOperationFeature> _batchOperationFeature;
        private readonly Lazy<DataFieldStructure[]> _filteredDataFields;
        private readonly Lazy<DataFieldStructure> _mainAttribute;

        public DataListStructure(EntityName entityName, IEnumerable<IConfigFeature> features) : base(features)
        {
            _identity = new DataListStructureIdentity(entityName);
            _displayNameLocalizedFeature = new Lazy<DisplayNameLocalizedFeature>(() => ElementFeatures.OfType<DisplayNameLocalizedFeature>().SingleOrDefault());
            _defaultFilterFeature = new Lazy<DefaultFilterFeature>(() => ElementFeatures.OfType<DefaultFilterFeature>().SingleOrDefault());
            _sortDescendingFeature = new Lazy<SortDescendingFeature>(() => ElementFeatures.OfType<SortDescendingFeature>().SingleOrDefault());
            _dataFieldsFeature = new Lazy<DataFieldsFeature>(() => ElementFeatures.OfType<DataFieldsFeature>().SingleOrDefault());
            _batchOperationFeature = new Lazy<BatchOperationFeature>(() => ElementFeatures.OfType<BatchOperationFeature>().SingleOrDefault());

            _filteredDataFields = new Lazy<DataFieldStructure[]>(() => DataFields.Where(x => x.IsFiltered).ToArray());
            _mainAttribute = new Lazy<DataFieldStructure>(() => DataFields.Single(x => x.IsMainAttribute));
        }

        public override IConfigElementIdentity ElementIdentity
        {
            get { return _identity; }
        }

        public override DataListStructureIdentity Identity
        {
            get { return _identity; }
        }

        public bool HasOperations
        {
            get { return _batchOperationFeature.Value != null && _batchOperationFeature.Value.OperationFeatures.Count != 0; }
        }

        public IEnumerable<IBoundOperationFeature> OperationFeatures
        {
            get { return _batchOperationFeature.Value != null ? _batchOperationFeature.Value.OperationFeatures : Enumerable.Empty<IBoundOperationFeature>(); }
        }

        public IEnumerable<DataFieldStructure> DataFields
        {
            get { return _dataFieldsFeature.Value != null ? _dataFieldsFeature.Value.DataFields : Enumerable.Empty<DataFieldStructure>(); }
        }

        public IEnumerable<DataFieldStructure> FilteredDataFields
        {
            get { return _filteredDataFields.Value; }
        }

        public string NameLocalizedResourceKey
        {
            get { return _displayNameLocalizedFeature.Value != null ? _displayNameLocalizedFeature.Value.ResourceKey : null; }
        }

        public Type NameLocalizedResourceManagerType
        {
            get { return _displayNameLocalizedFeature.Value != null ? _displayNameLocalizedFeature.Value.ResourceManagerType : null; }
        }

        public bool IsDefaultFilter
        {
            get { return _defaultFilterFeature != null; }
        }

        public bool IsSortDescending
        {
            get { return _sortDescendingFeature != null; }
        }

        public DataFieldStructure MainAttribute
        {
            get { return _mainAttribute.Value; }
        }
    }
}