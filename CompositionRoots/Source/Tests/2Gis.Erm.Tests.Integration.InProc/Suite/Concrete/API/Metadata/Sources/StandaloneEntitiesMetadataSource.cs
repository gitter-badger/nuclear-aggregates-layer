using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Domain.Entities;
using NuClear.Metamodeling.Provider.Sources;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Metadata.Sources
{
    public sealed partial class StandaloneEntitiesMetadataSource : MetadataSourceBase<MetadataEntitiesIdentity>
    {
        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get 
            { 
                return new Dictionary<Uri, IMetadataElement> 
                {
                    { _autonomousMetadataMetadata.Identity.Id, _autonomousMetadataMetadata },
                    { _sparseMetadataMetadata.Identity.Id, _sparseMetadataMetadata },
                    { _sparseMetadataMetadataFeatures.Identity.Id, _sparseMetadataMetadataFeatures }
                }; 
            }
        }
    }
}
