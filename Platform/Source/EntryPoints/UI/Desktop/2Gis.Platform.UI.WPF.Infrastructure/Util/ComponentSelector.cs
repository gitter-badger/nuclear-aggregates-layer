using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util
{
    /// <summary>
    /// Selector UI для компонентов - получает template специфического UI для представления специфических данных конкретного layout компонента
    /// </summary>
    public sealed class ComponentSelector<TLayoutComponent, TViewModelIndicator> : DataTemplateSelector
        where TLayoutComponent : ILayoutComponent
        where TViewModelIndicator : ILayoutComponentViewModel
    {
        private readonly Type _viewModelIndicator = typeof(TViewModelIndicator);
        private readonly IDictionary<Type, DataTemplate> _staticViewModelViewCache = new Dictionary<Type, DataTemplate>();
        private readonly IDictionary<Tuple<Type, Type>, DataTemplate> _dynamicViewModelViewCache = new Dictionary<Tuple<Type, Type>, DataTemplate>();
        
        private IReadOnlyDictionary<Type, TLayoutComponent> _components;

        public ComponentSelector(IEnumerable<TLayoutComponent> components)
        {
            UpdateComponents(components);
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return base.SelectTemplate(null, container);
            }

            var targetViewModelType = item.GetType();
            if (!_viewModelIndicator.IsAssignableFrom(targetViewModelType))
            {
                // throw new NotSupportedException("Only type assignable to " + _viewModelIndicator + " should be specified");
                return base.SelectTemplate(null, container);
            }

            TLayoutComponent component;
            if (!_components.TryGetValue(targetViewModelType, out component))
            {
                throw new InvalidOperationException("Can't get component description for item type: " + targetViewModelType);
            }

            DataTemplate resultTemplate;
            if (!typeof(IDynamicPropertiesContainer).IsAssignableFrom(targetViewModelType))
            {
                if (_staticViewModelViewCache.TryGetValue(targetViewModelType, out resultTemplate))
                {
                    return resultTemplate;
                }

                var viewType = component.ViewResolver.ResolveView((TViewModelIndicator)item);
                resultTemplate = TemplateUtils.CreateDataTemplate(targetViewModelType, viewType);
                _staticViewModelViewCache[targetViewModelType] = resultTemplate;
            }
            else
            {
                var viewType = component.ViewResolver.ResolveView((TViewModelIndicator)item);
                
                var viewModelViewTuple = Tuple.Create(targetViewModelType, viewType);
                if (_dynamicViewModelViewCache.TryGetValue(viewModelViewTuple, out resultTemplate))
                {
                    return resultTemplate;
                }

                resultTemplate = TemplateUtils.CreateDataTemplate(targetViewModelType, viewType);
                _dynamicViewModelViewCache[viewModelViewTuple] = resultTemplate;
            }

            return resultTemplate;
        }

        private void UpdateComponents(IEnumerable<TLayoutComponent> components)
        {
            _components = components.ToDictionary(component => component.ViewModelType);
        }
    }
}
