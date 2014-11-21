using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features
{
    public interface IEntityRelationFeature : IMetadataFeature
    {
        Type EntityType { get; }
        Type DocumentType { get; }
    }
}