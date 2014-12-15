using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features
{
    public sealed class DisableExpressionFeature<T> : ExpressionFeatureAbstract<T>, IDisableExpressionFeature
        where T : IViewModelAbstract
    {
        public DisableExpressionFeature(Expression<Func<T, bool>> expression) : base(expression)
        {
        }
    }
}