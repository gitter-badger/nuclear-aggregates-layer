using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources
{
    public abstract class MetadataSourceBase<TMetadataKindIdentity> : IMetadataSource<TMetadataKindIdentity> 
        where TMetadataKindIdentity : class, IMetadataKindIdentity, new()
    {
        private readonly IMetadataKindIdentity _metadataKindIdentity = new TMetadataKindIdentity();

        public IMetadataKindIdentity Kind
        {
            get { return _metadataKindIdentity; }
        }

        public abstract IReadOnlyDictionary<Uri, IMetadataElement> Metadata { get; }
    }
}