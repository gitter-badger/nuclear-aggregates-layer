using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.RelatedItems
{
    public sealed class RelatedItemsFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, IRelatedItemsHost, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, IRelatedItemsHost
    {
        public RelatedItemsFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        private IResourceDescriptor NameDescriptor { get; set; }

        private ITitleDescriptor TitleDescriptor { get; set; }

        public RelatedItemsFeatureAspect<TBuilder, TMetadataElement> Name<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            this.NameDescriptor = StringResourceDescriptor.Create(resourceKeyExpression);
            return this;
        }

        public RelatedItemsFeatureAspect<TBuilder, TMetadataElement> Name(string value)
        {
            this.NameDescriptor = new StaticStringResourceDescriptor(value);
            return this;
        }

        public RelatedItemsFeatureAspect<TBuilder, TMetadataElement> Title<TKey>(Expression<Func<TKey>> resourceKeyExpression)
        {
            this.TitleDescriptor = ResourceTitleDescriptor.Create(resourceKeyExpression);
            return this;
        }

        public RelatedItemsFeatureAspect<TBuilder, TMetadataElement> Title(string value)
        {
            this.TitleDescriptor = new StaticTitleDescriptor(value);
            return this;
        }


        public TBuilder Attach(params UIElementMetadata[] relatedItems)
        {
            if (relatedItems == null || relatedItems.Length == 0)
            {
                return AspectHostBuilder;
            }

            AspectHostBuilder.WithFeatures(new RelatedItemsFeature(NameDescriptor, TitleDescriptor, relatedItems));
            return AspectHostBuilder;
        }
    }
}
