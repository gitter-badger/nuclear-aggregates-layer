using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public class GridMetadata : MetadataElement<GridMetadata, GridMetadataBuilder>
    {
        private readonly EntityName _entity;
        private readonly IMetadataElementIdentity _identity;
        public GridMetadata(EntityName entity,  IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _entity = entity;
            _identity = IdBuilder.For<MetadataGridsIdentity>(_entity.ToString()).AsIdentity();
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public IEnumerable<DataListMetadata> DataLists
        {
            get { return this.Feature<AttachedDataListsFeature>().DataLists; }
        }

        public EntityName Entity
        {
            get { return _entity; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotImplementedException();
        }
    }
}