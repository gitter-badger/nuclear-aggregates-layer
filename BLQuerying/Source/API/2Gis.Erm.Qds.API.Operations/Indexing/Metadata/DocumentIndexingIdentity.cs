using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata
{
    public sealed class DocumentIndexingIdentity : MetadataKindIdentityBase<DocumentIndexingIdentity>
    {
        private readonly Uri _id = IdBuilder.For("Search/Indexing");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Qds indexing document metadata"; }
        }
    }
}