using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Platform.UI.WPF.Infrastructure.Util;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils
{
    public sealed class ViewModel2ViewMappingsSelector : DataTemplateSelector
    {
        private readonly IDictionary<Type,Type> _mappings;
        private readonly IDictionary<Type, DataTemplate> _cache = new Dictionary<Type, DataTemplate>();

        public ViewModel2ViewMappingsSelector(IEnumerable<IViewModelViewTypeMapping> mappings)
        {
            _mappings = mappings.ToDictionary(mapping => mapping.ViewModelType, mapping => mapping.ViewType);
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return base.SelectTemplate(null, container);
            }

            var targetViewModelType = item.GetType();
            Type resolvedViewType;
            DataTemplate resultTemplate;
            if (_cache.TryGetValue(targetViewModelType, out resultTemplate))
            {
                return resultTemplate;
            }

            if (!_mappings.TryGetValue(targetViewModelType, out resolvedViewType))
            {
                return base.SelectTemplate(item, container);
            }

            resultTemplate = TemplateUtils.CreateDataTemplate(targetViewModelType, resolvedViewType);
            _cache[targetViewModelType] = resultTemplate;
            return resultTemplate;
        }
    }
}
