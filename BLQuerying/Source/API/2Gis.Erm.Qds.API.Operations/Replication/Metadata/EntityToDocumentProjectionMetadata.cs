using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities.Concrete;

namespace DoubleGis.Erm.Qds.API.Operations.Replication.Metadata
{
    public class EntityToDocumentProjectionMetadata : MetadataElement<EntityToDocumentProjectionMetadata, EntityToDocumentProjectionMetadataBuilder>
    {
        private readonly Type _documentType;
        private readonly IMetadataElementIdentity _identity;

        public EntityToDocumentProjectionMetadata(Type documentType, IEnumerable<IMetadataFeature> features) 
            : base(features)
        {
            _documentType = documentType;
            _identity = new MetadataElementIdentity(IdBuilder.For<EntityToDocumentProjectionsIdentity>(documentType.Name));
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public Type DocumentType
        {
            get { return _documentType; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new System.NotImplementedException();
        }
    }
}