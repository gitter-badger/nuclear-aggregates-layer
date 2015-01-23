using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Concrete;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata
{
    public sealed class DocumentRelationMetadata : MetadataElement<DocumentRelationMetadata, DocumentRelationMetadataBuilder>
    {
        private readonly Type _documentType;
        private readonly IMetadataElementIdentity _identity;

        public DocumentRelationMetadata(Type documentType, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _documentType = documentType;
            _identity = new MetadataElementIdentity(IdBuilder.For<DocumentRelationsIdentity>(documentType.Name));
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
            throw new NotImplementedException();
        }
    }
}