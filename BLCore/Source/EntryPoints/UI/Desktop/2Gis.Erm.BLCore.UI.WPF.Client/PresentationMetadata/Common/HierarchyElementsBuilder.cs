using System.Windows;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Metadata.Common;

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
             EntityName entityName)
        {
            builderAspect.ShowGrid<HierarchyMetadataBuilder, HierarchyMetadata>(entityName);
            return builderAspect.AspectHostBuilder;
        }
    }
}
