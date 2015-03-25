using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

using NuClear.Metamodeling.Domain.Elements.Aspects.Features.Operations;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataListMetadata : MetadataElement<DataListMetadata, DataListMetadataBuilder>, IOperationsBoundElement
    {
        private readonly IEntityType _entity;
        private readonly string _concreteListing;
        private readonly IMetadataElementIdentity _identity;

        public DataListMetadata(IEntityType entity, string concreteListing, IEnumerable<IMetadataFeature> features) 
            : base(features)
        {
            _entity = entity;
            _concreteListing = concreteListing;
            _identity = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataListingsIdentity>(entity.ToString(), concreteListing).Build().AsIdentity();
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public IEntityType Entity
        {
            get { return _entity; }
        }

        public string ConcreteListing
        {
            get { return _concreteListing; }
        }

        public bool HasOperations
        {
            get
            {
                var feature = this.Features<OperationsSetFeature>().SingleOrDefault();
                return feature != null && feature.OperationFeatures.Any();
            }
        }

        public IEnumerable<OperationFeature> OperationFeatures
        {
            get
            {
                var feature = this.Features<OperationsSetFeature>().SingleOrDefault();
                return feature != null ? feature.OperationFeatures : Enumerable.Empty<OperationFeature>();
            }
        }

        public IEnumerable<DataFieldMetadata> DataFields
        {
            get
            {
                var feature = this.Features<DataFieldsFeature>().SingleOrDefault();
                return feature != null ? feature.DataFields : Enumerable.Empty<DataFieldMetadata>();
            }
        }

        public IEnumerable<DataFieldMetadata> FilteredDataFields
        {
            get { return DataFields.Where(x => x.IsFiltered); }
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

        public bool IsDefaultFilter
        {
            get
            {
                return this.Features<DefaultFilterFeature>().SingleOrDefault() != null;
            }
        }

        public bool IsSortDescending
        {
            get { return this.Features<SortDescendingFeature>().SingleOrDefault() != null; }
        }

        public DataFieldMetadata MainAttribute
        {
            get { return DataFields.Single(x => x.IsMainAttribute); }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotImplementedException();
        }
    }
}