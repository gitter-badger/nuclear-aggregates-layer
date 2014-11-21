﻿
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features.Parts
{
    // TODO {all, 20.11.2014}: Перекликается с реализацией для WPF. При возобновлении работы над WPF. Код нужно будет как-то объединить.
    public sealed class PartFeature : IPartFeature
    {
        private readonly ITitleDescriptor _titleDescriptor;
        private readonly IResourceDescriptor _nameDescriptor;

        public PartFeature(ITitleDescriptor titleDescriptor, IResourceDescriptor nameDescriptor)
        {
            _titleDescriptor = titleDescriptor;
            _nameDescriptor = nameDescriptor;
        }

        public string Title { get; private set; }
        public string Name { get; private set; }
    }
}