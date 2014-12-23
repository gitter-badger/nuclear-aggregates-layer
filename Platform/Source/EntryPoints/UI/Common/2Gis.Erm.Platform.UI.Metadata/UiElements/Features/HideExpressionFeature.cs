using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    public sealed class HideExpressionFeature<T> : ExpressionFeatureAbstract<T>, IHideExpressionFeature
        where T : IViewModelAbstract
    {
        public HideExpressionFeature(Expression<Func<T, bool>> expression) : base(expression)
        {
        }
    }
}