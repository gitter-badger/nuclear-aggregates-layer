using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Hierarchy
{
    public static class HierarchyElementsBuilderExtensions
    {
        public static HierarchyElementsBuilder BindMVVM<TViewModel, TView>(this HierarchyElementsBuilder builder) 
            where TViewModel : class, IViewModel where TView : class, IView
        {
            builder.Features.Add(new ViewModelViewMappingFeature<TViewModel, TView>());
            return builder;
        }
    }
}

