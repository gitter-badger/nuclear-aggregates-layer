using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Common
{
    public sealed class ResourceDictionaryImageProvider : IImageProvider
    {
        private readonly IEnumerable<ResourceDictionary> _resourceDictionaries;
        private readonly ResourceKey _resourceKey;
        private readonly Lazy<ImageSource> _source;

        public ResourceDictionaryImageProvider(IEnumerable<ResourceDictionary> resourceDictionaries, ResourceKey resourceKey)
        {
            _resourceDictionaries = resourceDictionaries;
            _resourceKey = resourceKey;
            _source = new Lazy<ImageSource>(ResolveImage);
        }

        public ImageSource Source 
        {
            get
            {
                return _source.Value;
            }
        }

        private ImageSource ResolveImage()
        {
            foreach (var dictionary in _resourceDictionaries)
            {
                if (!dictionary.Contains(_resourceKey))
                {
                    continue;
                }

                return (ImageSource)dictionary[_resourceKey];
            }

            throw new InvalidOperationException("Can't find specified resource with key " + _resourceKey + " in specified resource dictionaries set" );
        }
    }
}