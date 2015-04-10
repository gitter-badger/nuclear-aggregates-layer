using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public class GridMetadata : MetadataElement<GridMetadata, GridMetadataBuilder>
    {
        private readonly IEntityType _entity;
        private readonly IMetadataElementIdentity _identity;
        public GridMetadata(IEntityType entity,  IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _entity = entity;
            _identity = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataGridsIdentity>(_entity.Description).Build().AsIdentity();
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public IEnumerable<DataListMetadata> DataLists
        {
            get { return this.Feature<AttachedDataListsFeature>().DataLists; }
        }

        public IEntityType Entity
        {
            get { return _entity; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotImplementedException();
        }
    }
}