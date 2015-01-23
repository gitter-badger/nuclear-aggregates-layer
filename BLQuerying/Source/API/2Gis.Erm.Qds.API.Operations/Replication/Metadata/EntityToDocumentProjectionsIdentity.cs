using System;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.Qds.API.Operations.Replication.Metadata
{
    public class EntityToDocumentProjectionsIdentity : MetadataKindIdentityBase<EntityToDocumentProjectionsIdentity>
    {
        private readonly Uri _id = IdBuilder.For("Search/Replication/Projections");
        
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