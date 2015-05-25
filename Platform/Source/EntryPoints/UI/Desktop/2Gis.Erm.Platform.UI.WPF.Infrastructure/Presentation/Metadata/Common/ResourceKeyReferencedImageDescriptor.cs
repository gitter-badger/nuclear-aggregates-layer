using System;
using System.Globalization;
using System.Windows;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Images;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Metadata.Common
{
    public sealed class ResourceKeyReferencedImageDescriptor : IImageDescriptor
    {
        private readonly ResourceKey _resourceKey;

        public ResourceKeyReferencedImageDescriptor(ResourceKey resourceKey)
        {
            _resourceKey = resourceKey;
        }

        public ResourceKey ResourceKey
        {
            get { return _resourceKey; }
        }

        public object GetValue(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public string ResourceKeyToString()
        {
            return ResourceKey.ToString();
        }
    }
}
