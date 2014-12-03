using System;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public sealed class ViewModelTypedViewMapping<TViewModel, TView> : IViewModelViewTypeMapping<TViewModel, TView>
        where TViewModel : class, IViewModel where TView : class, IView
    {
        private readonly static Lazy<ViewModelTypedViewMapping<TViewModel, TView>> SingleInstance =
            new Lazy<ViewModelTypedViewMapping<TViewModel, TView>>(() => new ViewModelTypedViewMapping<TViewModel, TView>());

        public static ViewModelTypedViewMapping<TViewModel, TView> Instance
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

        public string ViewName
        {
            get { return ViewType.FullName; }
        }
    }
}