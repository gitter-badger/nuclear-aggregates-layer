using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public sealed class ViewModelViewMappingFeature<TViewModel> : IViewModelViewMappingFeature
        where TViewModel : class, IViewModelAbstract
    {
        public ViewModelViewMappingFeature(IViewModelViewMapping mapping)
        {
            Mapping = mapping;
        }

        public IViewModelViewMapping Mapping { get; private set; }
    }

    public sealed class ViewModelViewMappingFeature<TViewModel, TView> : IViewModelViewMappingFeature
        where TViewModel : class, IViewModel
        where TView : class, IView
    {
        private readonly IViewModelViewTypeMapping<TViewModel, TView> _mapping = new ViewModelTypedViewMapping<TViewModel, TView>();

        public IViewModelViewMapping Mapping
        {
            get
            {
                return _mapping;
            }
        }
    }
}