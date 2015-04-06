using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Name
{
    // TODO {all, 21.11.2014}: при необходимости объединить с TitleFeatureAspect
    public sealed class NameFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, INamedElement, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, INamedElement
    {
        public NameFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Resource<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            AspectHostBuilder.WithFeatures(new NameFeature(ResourceTitleDescriptor.Create(resourceKeyExpression)));
            return AspectHostBuilder;
        }

        public TBuilder Static(string title)
        {
            AspectHostBuilder.WithFeatures(new NameFeature(new StaticTitleDescriptor(title)));
            return AspectHostBuilder;
        }
    }
}