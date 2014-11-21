using System;
using System.Windows.Controls;

namespace DoubleGis.UI.WPF.Infrastructure.Modules.Layout.LayoutRegions.Navigation
{
    public sealed class InstanceIndependentLayoutNavigationComponent<TViewModel, TView> : ILayoutNavigationComponent
        where TViewModel : class, INavigationArea
        where TView : UserControl
    {
        public InstanceIndependentLayoutNavigationComponent()
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