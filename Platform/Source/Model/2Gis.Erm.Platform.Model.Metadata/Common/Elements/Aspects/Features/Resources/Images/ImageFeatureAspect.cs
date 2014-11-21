using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images
{
    public sealed class ImageFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, IImageBoundElement, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, IImageBoundElement
    {
        public ImageFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Resource<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            AspectHostBuilder.WithFeatures(new ImageFeature(ResourceImageDescriptor.Create(resourceKeyExpression)));
            return AspectHostBuilder;
        }

        public TBuilder Path(string staticPath)
        {
            AspectHostBuilder.WithFeatures(new ImageFeature(new StaticPathImageDescriptor(staticPath)));
            return AspectHostBuilder;
        }
    }
}