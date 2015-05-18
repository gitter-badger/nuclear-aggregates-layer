using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features
{
    public interface IDocumentPartFeature : IMetadataFeature
    {
        Type DocumentPartType { get; }
    }
}