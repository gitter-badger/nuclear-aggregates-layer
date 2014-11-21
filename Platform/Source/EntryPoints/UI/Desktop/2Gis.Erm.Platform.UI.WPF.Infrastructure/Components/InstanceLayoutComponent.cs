using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewResolvers;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Components
{
    public abstract class InstanceLayoutComponent<TRegionComponentIndicator, TViewModelIndicator, TViewModel, TViewModelIdentity>
        : ILayoutComponent<TViewModelIndicator>
        where TRegionComponentIndicator : ILayoutComponent<TViewModelIndicator>
        where TViewModelIndicator : ILayoutComponentViewModel
        where TViewModel : class, TViewModelIndicator, IViewModel<TViewModelIdentity> 
        where TViewModelIdentity : IViewModelIdentity
    {
        private readonly Lazy<ILayoutComponentViewResolver> _viewResolver;
        
        protected InstanceLayoutComponent()
        {
            ComponentId = Guid.NewGuid();
            ViewModelType = typeof(TViewModel);
            _viewResolver = new Lazy<ILayoutComponentViewResolver>(CreateViewResolver);
        }

        protected abstract ILayoutComponentViewResolver CreateViewResolver();

        public Guid ComponentId { get; private set; }
        public Type ViewModelType { get; private set; }

        public ILayoutComponentViewResolver ViewResolver
        {
            get
            {
                return _viewResolver.Value;
            }
        }
    } 
}