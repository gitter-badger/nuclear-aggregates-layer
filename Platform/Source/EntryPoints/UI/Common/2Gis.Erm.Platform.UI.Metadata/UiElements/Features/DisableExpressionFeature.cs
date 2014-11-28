using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features
{
    public sealed class DisableExpressionFeature<T> : IDisableExpressionFeature
        where T : IViewModelAbstract
    {
        public DisableExpressionFeature(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
        }

        public Expression<Func<T, bool>> Expression { get; private set; }
    }
}