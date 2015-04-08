using NuClear.Metamodeling.Elements;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap
{
    public sealed class ViewModelViewMappingAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, IBoundViewModelView, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, IBoundViewModelView
    {
        public ViewModelViewMappingAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Bind<TViewModel, TView>()
            where TViewModel : class, IViewModel
            where TView : class, IView
        {
            AspectHostBuilder.WithFeatures(new ViewModelViewMappingFeature<TViewModel, TView>());
            return AspectHostBuilder;
        }

        public TBuilder Bind<TViewModel>(string viewPath)
            where TViewModel : class, IViewModelAbstract
        {
            AspectHostBuilder.WithFeatures(new ViewModelViewMappingFeature<TViewModel>(new ViewModelViewPathMapping<TViewModel>(viewPath)));
            return AspectHostBuilder;
        }
    }
}
