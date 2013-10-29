using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles
{
    public sealed class TitleFeatureAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, ITitledElement, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : ConfigElement, ITitledElement
    {
        public TitleFeatureAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Resource<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            AspectHostBuilder.Features.Add(new TitleFeature(ResourceTitleDescriptor.Create(resourceKeyExpression))); ;
            return AspectHostBuilder;
        }

        public TBuilder Static(string title)
        {
            AspectHostBuilder.Features.Add(new TitleFeature(new StaticTitleDescriptor(title))); ;
            return AspectHostBuilder;

        }
    }
}