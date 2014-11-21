using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public sealed class ViewModelViewMappingFeature<TViewModel, TView> : IViewModelViewMappingFeature
        where TViewModel : class, IViewModel
        where TView : class, IView
    {
        private readonly IViewModelViewMapping<TViewModel, TView> _mapping = new ViewModelViewMapping<TViewModel, TView>();

        public IViewModelViewMapping Mapping
        {
            get
            {
                return _mapping;
            }
        }
    }
}