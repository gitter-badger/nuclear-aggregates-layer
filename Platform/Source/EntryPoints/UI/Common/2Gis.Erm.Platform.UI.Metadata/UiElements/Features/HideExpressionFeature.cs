using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    public sealed class HideExpressionFeature<T> : ExpressionFeatureAbstract<T>, IHideExpressionFeature
        where T : IAspect
    {
        public HideExpressionFeature(Expression<Func<T, bool>> expression) : base(expression)
        {
        }
    }
}