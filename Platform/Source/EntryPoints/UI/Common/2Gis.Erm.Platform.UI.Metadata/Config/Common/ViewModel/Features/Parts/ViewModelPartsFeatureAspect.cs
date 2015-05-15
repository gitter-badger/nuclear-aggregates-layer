using System;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Metamodeling.Elements;
using NuClear.ResourceUtilities;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts
{
    public sealed class ViewModelPartsFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, IPartsContainerElement, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement, IPartsContainerElement
    {
        public ViewModelPartsFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Use(params Expression<Func<object>>[] partKeyExpressions)
        {
            if (partKeyExpressions == null || partKeyExpressions.Length == 0)
            {
                return AspectHostBuilder;
            }

            AspectHostBuilder.WithFeatures(new ViewModelPartsFeature(partKeyExpressions.Select(ResourceEntryKey.Create).ToArray()));
            return AspectHostBuilder;
        }
    }
}
