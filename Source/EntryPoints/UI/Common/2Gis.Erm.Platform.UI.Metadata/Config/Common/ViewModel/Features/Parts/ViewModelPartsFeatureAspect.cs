using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts
{
    public sealed class ViewModelPartsFeatureAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, IPartsContainerElement, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : ConfigElement, IPartsContainerElement
    {
        public ViewModelPartsFeatureAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Use(params Expression<Func<object>>[] partKeyExpressions)
        {
            if (partKeyExpressions == null || partKeyExpressions.Length == 0)
            {
                return AspectHostBuilder;
            }

            AspectHostBuilder.Features.Add(new ViewModelPartsFeature(partKeyExpressions.Select(ResourceEntryKey.Create).ToArray()));
            return AspectHostBuilder;
        }
    }
}
