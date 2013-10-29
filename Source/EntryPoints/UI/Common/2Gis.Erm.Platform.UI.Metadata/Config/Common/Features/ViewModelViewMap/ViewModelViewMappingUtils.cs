﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public static class ViewModelViewMappingUtils
    {
        public static void ProcessMVVMMappings(this IConfigElement element, Dictionary<Type, IViewModelViewMapping> registry)
        {
            var feature = element.ElementFeatures.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
            if (feature != null)
            {
                IViewModelViewMapping alreadyExistingMapping;
                if (!registry.TryGetValue(feature.Mapping.ViewModelType, out alreadyExistingMapping))
                {
                    registry.Add(feature.Mapping.ViewModelType, feature.Mapping);
                }
                else
                {
                    if (alreadyExistingMapping.ViewType != feature.Mapping.ViewType)
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