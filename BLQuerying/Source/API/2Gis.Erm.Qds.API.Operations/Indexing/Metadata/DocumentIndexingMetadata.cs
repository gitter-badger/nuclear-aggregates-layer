using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities.Concrete;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata
{
    public sealed class DocumentIndexingMetadata : MetadataElement<DocumentIndexingMetadata, DocumentIndexingMetadataBuilder>
    {
        private readonly Type _documentType;
        private readonly IMetadataElementIdentity _identity;

        public DocumentIndexingMetadata(Type documentType, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _documentType = documentType;
            _identity = new MetadataElementIdentity(IdBuilder.For<DocumentIndexingIdentity>(documentType.Name));
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