using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Metadata.Common;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Images;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Common
{
    public sealed class ImageProviderFactory : IImageProviderFactory
    {
        private readonly ResourceDictionary[] _resources; 

        public ImageProviderFactory(IEnumerable<Uri> resourceDictionariesUri)
        {
            _resources = resourceDictionariesUri.Select(uri => new ResourceDictionary { Source = uri }).ToArray();
        }

        public IImageProvider Create(IImageDescriptor imageDescriptor)
        {
            if (imageDescriptor == null)
            {
                throw new ArgumentNullException("imageDescriptor");
            }

            var keyReferencedImageDescriptor = imageDescriptor as ResourceKeyReferencedImageDescriptor;
            if (keyReferencedImageDescriptor != null)
            {
                return new ResourceDictionaryImageProvider(_resources, keyReferencedImageDescriptor.ResourceKey);
            }

            throw new InvalidOperationException("Can't create image provider for specified desriptor. " + imageDescriptor.GetType().Name);
        }
    }
}