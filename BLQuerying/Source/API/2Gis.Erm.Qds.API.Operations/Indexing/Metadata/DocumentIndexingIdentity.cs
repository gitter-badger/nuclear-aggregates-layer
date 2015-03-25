using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata
{
    public sealed class DocumentIndexingIdentity : MetadataKindIdentityBase<DocumentIndexingIdentity>
    {
        private readonly Uri _id = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For("Search/Indexing");

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