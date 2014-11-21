using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public sealed class ViewModelViewMapping<TViewModel, TView> : IViewModelViewMapping<TViewModel, TView>
        where TViewModel : class, IViewModel where TView : class, IView
    {
        private readonly static Lazy<ViewModelViewMapping<TViewModel, TView>> SingleInstance =
            new Lazy<ViewModelViewMapping<TViewModel, TView>>(() => new ViewModelViewMapping<TViewModel, TView>());

        public static ViewModelViewMapping<TViewModel, TView> Instance
        {
            get
            {
                return SingleInstance.Value;
            }
        }

        public Type ViewType
        {
            get
            {
                return typeof(TView);
            }
        }
        
        public Type ViewModelType
        {
            get
            {
                return typeof(TViewModel);
            }
        }
    }
}