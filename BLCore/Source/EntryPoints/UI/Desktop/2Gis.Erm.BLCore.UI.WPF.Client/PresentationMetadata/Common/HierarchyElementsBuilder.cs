using System.Windows;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Metadata.Common;

using NuClear.Metamodeling.Domain.Elements.Aspects.Features.Handler;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Images;
using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common
{
    public static class HierarchyElementsBuilderExtensions
    {
        public static OldUIElementMetadataBuilder UseResourceIcon(this OldUIElementMetadataBuilder builder, ResourceKey iconResourceKey)
        {
            return builder.Icon.Resource(iconResourceKey);
        }

        public static OldUIElementMetadataBuilder Resource(this ImageFeatureAspect<OldUIElementMetadataBuilder, OldUIElementMetadata> builderAspect, ResourceKey iconResourceKey)
        {
            builderAspect.AspectHostBuilder.WithFeatures(new ImageFeature(new ResourceKeyReferencedImageDescriptor(iconResourceKey)));
            return builderAspect.AspectHostBuilder;
        }

        public static OldUIElementMetadataBuilder ShowGrid(
             this HandlerFeatureAspect<OldUIElementMetadataBuilder, OldUIElementMetadata> builderAspect,
             IEntityType entityName,
             string filterExpression,
             string disableExpression)
        {
            builderAspect.ShowGrid<OldUIElementMetadataBuilder, OldUIElementMetadata>(entityName, filterExpression, disableExpression);
            return builderAspect.AspectHostBuilder;
        }
    }
}
