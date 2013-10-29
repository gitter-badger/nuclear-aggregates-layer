using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public interface IViewModelViewMapping
    {
        Type ViewType { get; }
        Type ViewModelType { get; }
    }

    public interface IViewModelViewMapping<TViewModel, TView> : IViewModelViewMapping
        where TViewModel : class, IViewModel
        where TView : class, IView
    {
    }
}
