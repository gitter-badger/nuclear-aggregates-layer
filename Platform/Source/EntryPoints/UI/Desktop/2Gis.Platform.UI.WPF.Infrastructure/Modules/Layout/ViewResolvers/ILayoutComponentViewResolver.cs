using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewResolvers
{
    public interface ILayoutComponentViewResolver
    {
        Type ResolveView(ILayoutComponentViewModel viewModel);
    }
}
