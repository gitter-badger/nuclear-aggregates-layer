﻿
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards
{
    public sealed class ExtendedInfoFeature : IMetadataFeature
    {
        public ExtendedInfoFeature(IResourceDescriptor extendedInfo)
        {
            ExtendedInfo = extendedInfo;
        }

        public IResourceDescriptor ExtendedInfo { get; private set; }
    }
}