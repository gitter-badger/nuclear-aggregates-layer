using System.Windows;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common
{
    public static class HierarchyElementsBuilderExtensions
    {
        public static HierarchyElementsBuilder UseResourceIcon(this HierarchyElementsBuilder builder, ResourceKey iconResourceKey)
        {
            return builder.Icon.Resource(iconResourceKey);
        }

        public static HierarchyElementsBuilder Resource(this ImageFeatureAspect<HierarchyElementsBuilder, HierarchyElement> builderAspect, ResourceKey iconResourceKey)
        {
            builderAspect.AspectHostBuilder.Features.Add(new ImageFeature(new ResourceKeyReferencedImageDescriptor(iconResourceKey)));
            return builderAspect.AspectHostBuilder;
        }

        public static HierarchyElementsBuilder ShowGrid(
             this HandlerFeatureAspect<HierarchyElementsBuilder, HierarchyElement> builderAspect,
             EntityName entityName,
             string filterExpression,
             string disableExpression)
        {
            builderAspect.ShowGrid<HierarchyElementsBuilder, HierarchyElement>(entityName, filterExpression, disableExpression);
            return builderAspect.AspectHostBuilder;
        }
    }
}
