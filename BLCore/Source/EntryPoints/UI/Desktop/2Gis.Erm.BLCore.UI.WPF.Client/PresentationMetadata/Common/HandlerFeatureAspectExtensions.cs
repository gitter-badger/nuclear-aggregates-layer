using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Grid;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common
{
    public static class HandlerFeatureAspectExtensions
    {
        public static TBuilder ShowGrid<TBuilder, TElement>(
            this HandlerFeatureAspect<TBuilder, TElement> builderAspect,
            EntityName entityName) 
            where TBuilder : MetadataElementBuilder<TBuilder, TElement>, new() 
            where TElement : MetadataElement, IHandlerBoundElement
        {
            builderAspect.ShowGridByConvention(entityName);
            builderAspect.AspectHostBuilder.WithFeatures(new ViewModelViewMappingFeature<GridViewModel, GridView>());
            return builderAspect.AspectHostBuilder;
        }
    }
}