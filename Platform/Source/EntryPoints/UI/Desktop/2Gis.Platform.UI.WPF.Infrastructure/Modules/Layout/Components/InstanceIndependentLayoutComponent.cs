using System;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewResolvers;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components
{
    /// <summary>
    /// Класс описатель компонента, позволяет сопоставить конкретный тип viewmodel с конкретным типом view (независимо от свойств конкретного экземпляра viewmodel)
    /// Также обеспечивает привязку связки viewmodel-view с конкретным layout регионом размещения
    /// </summary>
    /// <typeparam name="TRegionComponentIndicator">Индикатор (базовый интерфейс) целевого layout региона для компонента</typeparam>
    /// <typeparam name="TViewModelIndicator">Индикатор (базовый интерфейс) viewmodel специфичной для конкретного layout региона</typeparam>
    /// <typeparam name="TViewModel">Конкретный тип viewmodel привязанный к компоненту</typeparam>
    /// <typeparam name="TView">Конкретный тип view привязанный к компоненту</typeparam>
    public abstract class InstanceIndependentLayoutComponent<TRegionComponentIndicator, TViewModelIndicator, TViewModel, TView>
        : ILayoutComponent<TViewModelIndicator>
        where TRegionComponentIndicator : ILayoutComponent<TViewModelIndicator>
        where TViewModelIndicator : ILayoutComponentViewModel
        where TViewModel : class, TViewModelIndicator
        where TView : Control
    {
        protected InstanceIndependentLayoutComponent()
        {
            ComponentId = Guid.NewGuid();
            ViewModelType = typeof(TViewModel);
            ViewResolver = new InstanceIndependentLayoutComponentViewResolver<TViewModel, TView>();
        }

        public Guid ComponentId { get; private set; }
        public Type ViewModelType { get; private set; }
        public ILayoutComponentViewResolver ViewResolver { get; private set; }
    }
}