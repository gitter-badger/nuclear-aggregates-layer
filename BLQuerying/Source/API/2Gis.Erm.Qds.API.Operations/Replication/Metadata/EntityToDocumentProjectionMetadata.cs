using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Elements.Identities.Concrete;

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
            _identity = new MetadataElementIdentity(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<EntityToDocumentProjectionsIdentity>(documentType.Name));
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