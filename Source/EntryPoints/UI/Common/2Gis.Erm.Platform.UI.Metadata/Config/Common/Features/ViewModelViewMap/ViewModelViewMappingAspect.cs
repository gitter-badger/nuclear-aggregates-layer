using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public sealed class ViewModelViewMappingAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, IBoundViewModelView, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : ConfigElement, IBoundViewModelView
    {
        public ViewModelViewMappingAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Bind<TViewModel, TView>()
            where TViewModel : class, IViewModel
            where TView : class, IView
        {
            AspectHostBuilder.Features.Add(new ViewModelViewMappingFeature<TViewModel, TView>());
            return AspectHostBuilder;
        }
    }
}
