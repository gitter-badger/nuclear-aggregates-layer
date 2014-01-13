﻿using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Grid;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common
{
    public static class HandlerFeatureAspectExtensions
    {
        public static TBuilder ShowGrid<TBuilder, TElement>(
            this HandlerFeatureAspect<TBuilder, TElement> builderAspect,
            EntityName entityName,
            string filterExpression,
            string disableExpression) 
            where TBuilder : ConfigElementBuilder<TBuilder, TElement>, new() 
            where TElement : ConfigElement, IHandlerBoundElement
        {
            builderAspect.ShowGridByConvention(entityName, filterExpression, disableExpression);
            builderAspect.AspectHostBuilder.Features.Add(new ViewModelViewMappingFeature<GridViewModel, GridView>());
            return builderAspect.AspectHostBuilder;
        }
    }
}