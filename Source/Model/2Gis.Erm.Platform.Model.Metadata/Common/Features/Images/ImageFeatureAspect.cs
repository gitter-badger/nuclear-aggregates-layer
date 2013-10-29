using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Images
{
    public sealed class ImageFeatureAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, IImageBoundElement, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : ConfigElement, IImageBoundElement
    {
        public ImageFeatureAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Resource<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            AspectHostBuilder.Features.Add(new ImageFeature(ResourceImageDescriptor.Create(resourceKeyExpression))); ;
            return AspectHostBuilder;
        }

        public TBuilder Path(string staticPath)
        {
            AspectHostBuilder.Features.Add(new ImageFeature(new StaticPathImageDescriptor(staticPath))); ;
            return AspectHostBuilder;
        }
    }
}