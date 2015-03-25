using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Hierarchy
{
    public static class HierarchyElementsBuilderExtensions
    {
        public static OldUIElementMetadataBuilder BindMVVM<TViewModel, TView>(this OldUIElementMetadataBuilder builder) 
            where TViewModel : class, IViewModel where TView : class, IView
        {
            builder.WithFeatures(new ViewModelViewMappingFeature<TViewModel, TView>());
            return builder;
        }
    }
}
