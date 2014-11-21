using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewResolvers;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Components
{
    public sealed class InstanceLayoutComponentViewResolver<TViewModel, TViewModelIdentity> : ILayoutComponentViewResolver 
        where TViewModel : IViewModel<TViewModelIdentity> 
        where TViewModelIdentity : IViewModelIdentity
    {
        private readonly IEnumerable<Func<TViewModelIdentity, Type>> _conditionalResolvers;

        public InstanceLayoutComponentViewResolver(IEnumerable<Func<TViewModelIdentity, Type>> conditionalResolvers)
        {
            _conditionalResolvers = conditionalResolvers;
        }

        public Type ResolveView(ILayoutComponentViewModel viewModel)
        {
            var viewModelWithDescriptor = viewModel as IViewModel<TViewModelIdentity>;
            foreach (var resolver in _conditionalResolvers)
            {
                var viewType = resolver(viewModelWithDescriptor.ConcreteIdentity);
                if (viewType != null)
                {
                    return viewType;
                }
            }

            throw new InvalidOperationException("Can't resolve view by view model. Processing view model type: " + typeof(TViewModel));
        }
    }
}