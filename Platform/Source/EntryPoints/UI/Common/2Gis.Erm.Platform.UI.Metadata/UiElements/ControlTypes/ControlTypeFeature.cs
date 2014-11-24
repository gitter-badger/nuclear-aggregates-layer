﻿using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes
{
    public sealed class ControlTypeFeature : ISecuredElementFeature
    {
        public ControlTypeFeature(IResourceDescriptor controlTypeDescriptor)
        {
            ControlTypeDescriptor = controlTypeDescriptor;
        }

        public IResourceDescriptor ControlTypeDescriptor { get; private set; }
    }
}