using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Hierarchy
{
    public static class HierarchyElementsBuilderExtensions
    {
        public static HierarchyMetadataBuilder BindMVVM<TViewModel, TView>(this HierarchyMetadataBuilder builder) 
            where TViewModel : class, IViewModel where TView : class, IView
        {
            builder.WithFeatures(new ViewModelViewMappingFeature<TViewModel, TView>());
            return builder;
        }
    }
}
