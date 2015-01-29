using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public abstract class ExpressionsFeatureAspect<TBuilder, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement
    {
        protected TBuilder AspectHostBuilder { get; set; }
        protected IList<LambdaExpression> Expressions { get; private set; }

        protected ExpressionsFeatureAspect(TBuilder aspectHostBuilder)
        {
            AspectHostBuilder = aspectHostBuilder;
            Expressions = new List<LambdaExpression>();
        }

        public ExpressionsFeatureAspect<TBuilder, TMetadataElement> Func<TAspect>(Expression<Func<TAspect, bool>> expression)
            where TAspect : IAspect
        {
            Expressions.Add(expression);
            return this;
        }

        public abstract TBuilder Combine(LogicalOperation combination);
    }
}