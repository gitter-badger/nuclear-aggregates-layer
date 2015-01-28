using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public sealed class DisableExpressionFeature<T> : IDisableExpressionFeature
        where T : IAspect
    {
        public DisableExpressionFeature(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
        }

        public LambdaExpression Expression { get; private set; }
    }
}