using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features
{
    public interface IEntityRelationFeature : IMetadataFeature
    {
        Type EntityType { get; }
        Type DocumentType { get; }
    }
}