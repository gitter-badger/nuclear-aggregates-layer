using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public sealed class HideExpressionFeature<T> : IHideExpressionFeature
        where T : IAspect
    {
        public HideExpressionFeature(Expression<Func<T, bool>> expression) 
        {
            Expression = expression;
        }

        public LambdaExpression Expression { get; private set; }
    }
}