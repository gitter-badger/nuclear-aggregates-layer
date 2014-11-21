using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources
{
    public interface IMetadataSource
    {
        IMetadataKindIdentity Kind { get; }
        IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }

    public interface IMetadataSource<TMetadataKindIdentity> : IMetadataSource
        where TMetadataKindIdentity : class, IMetadataKindIdentity, new()
    {
    }
}
