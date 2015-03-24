using System.Windows;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Metadata.Common;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Handler;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Images;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common
{
    public static class HierarchyElementsBuilderExtensions
    {
        public static HierarchyMetadataBuilder UseResourceIcon(this HierarchyMetadataBuilder builder, ResourceKey iconResourceKey)
        {
            return builder.Icon.Resource(iconResourceKey);
        }

        public static HierarchyMetadataBuilder Resource(this ImageFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> builderAspect, ResourceKey iconResourceKey)
        {
            builderAspect.AspectHostBuilder.WithFeatures(new ImageFeature(new ResourceKeyReferencedImageDescriptor(iconResourceKey)));
            return builderAspect.AspectHostBuilder;
        }

        public static HierarchyMetadataBuilder ShowGrid(
             this HandlerFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> builderAspect,
             IEntityType entityName,
             string filterExpression,
             string disableExpression)
        {
            builderAspect.ShowGrid<HierarchyMetadataBuilder, HierarchyMetadata>(entityName, filterExpression, disableExpression);
            return builderAspect.AspectHostBuilder;
        }
    }
}
