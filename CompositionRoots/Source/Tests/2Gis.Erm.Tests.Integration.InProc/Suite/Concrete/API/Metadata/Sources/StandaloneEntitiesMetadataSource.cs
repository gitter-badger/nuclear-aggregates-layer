using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider.Sources;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;

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
