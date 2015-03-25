using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.Qds.API.Operations.Replication.Metadata
{
    public class EntityToDocumentProjectionsIdentity : MetadataKindIdentityBase<EntityToDocumentProjectionsIdentity>
    {
        private readonly Uri _id = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For("Search/Replication/Projections");
        
        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Entity replication for indexing metadata"; }
        }
    }
}