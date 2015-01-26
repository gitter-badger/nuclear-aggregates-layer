﻿using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Grid;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features.Handler;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common
{
    public static class HandlerFeatureAspectExtensions
    {
        public static TBuilder ShowGrid<TBuilder, TElement>(
            this HandlerFeatureAspect<TBuilder, TElement> builderAspect,
            IEntityType entityName,
            string filterExpression,
            string disableExpression) 
            where TBuilder : MetadataElementBuilder<TBuilder, TElement>, new() 
            where TElement : MetadataElement, IHandlerBoundElement
        {
            builderAspect.ShowGridByConvention(entityName, filterExpression, disableExpression);
            builderAspect.AspectHostBuilder.WithFeatures(new ViewModelViewMappingFeature<GridViewModel, GridView>());
            return builderAspect.AspectHostBuilder;
        }
    }
}