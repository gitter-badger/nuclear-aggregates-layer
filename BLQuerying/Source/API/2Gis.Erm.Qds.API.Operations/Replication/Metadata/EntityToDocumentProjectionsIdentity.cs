using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace DoubleGis.Erm.Qds.API.Operations.Replication.Metadata
{
    public class EntityToDocumentProjectionsIdentity : MetadataKindIdentityBase<EntityToDocumentProjectionsIdentity>
    {
        private readonly Uri _id = MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "Search/Replication/Projections");
        
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