using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public static class ViewModelViewMappingUtils
    {
        public static void ProcessMVVMMappings(this IMetadataElement element, Dictionary<Type, IViewModelViewTypeMapping> registry)
        {
            var feature = element.Features.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
            if (feature != null)
            {
                IViewModelViewTypeMapping alreadyExistingMapping;
                if (!registry.TryGetValue(feature.Mapping.ViewModelType, out alreadyExistingMapping))
                {
                    registry.Add(feature.Mapping.ViewModelType, (IViewModelViewTypeMapping)feature.Mapping);
                }
                else
                {
                    if (alreadyExistingMapping.ViewType != ((IViewModelViewTypeMapping)feature.Mapping).ViewType)
                    {
                        throw new InvalidOperationException("For the same view model type " + alreadyExistingMapping.ViewModelType + " specified several different view types");
                    }
                }
            }

            if (element.Elements != null)
            {
                foreach (var subElement in element.Elements)
                {
                    subElement.ProcessMVVMMappings(registry);
                }
            }
        }
    }
}