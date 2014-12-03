using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public interface IViewModelViewMapping
    {
        string ViewName { get; }
        Type ViewModelType { get; }
    }

    // Над этим нужно дополнительно подумать. Сейчас была цель не разломать WPF и использовать маппинг в Web.MVC
    public interface IViewModelViewTypeMapping : IViewModelViewMapping
    {
        Type ViewType { get; }        
    }

    public interface IViewModelViewTypeMapping<TViewModel, TView> : IViewModelViewTypeMapping
        where TViewModel : class, IViewModel
        where TView : class, IView
    {
    }
}
