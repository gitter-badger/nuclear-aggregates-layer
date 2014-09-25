using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features
{
    public interface IDocumentPartFeature : IMetadataFeature
    {
        Type DocumentPartType { get; }
    }
}