using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewResolvers
{
    public sealed class InstanceIndependentLayoutComponentViewResolver<TViewModel, TView> : ILayoutComponentViewResolver
        where TViewModel : ILayoutComponentViewModel
    {
        private readonly Type _viewModelType = typeof(TViewModel);
        private readonly Type _viewType = typeof(TView);

        public Type ResolveView(ILayoutComponentViewModel viewModel)
        {
            var processingType = viewModel.GetType();
            if (_viewModelType != processingType)
            {
                throw new InvalidOperationException(string.Format("View model invalid type. Expected: {0}. Received: {1}.", _viewModelType, processingType));
            }

            return _viewType;
        }
    }
}