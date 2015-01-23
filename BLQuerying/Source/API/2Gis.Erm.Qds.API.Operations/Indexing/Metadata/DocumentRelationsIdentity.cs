﻿using System;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata
{
    public sealed class DocumentRelationsIdentity : MetadataKindIdentityBase<DocumentRelationsIdentity>
    {
        private readonly Uri _id = IdBuilder.For("Search/Indexing/DocumentRelations");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Qds indexing document relations metadata"; }
        }
    }
}