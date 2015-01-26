using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    public sealed class DisableExpressionFeature<T> : ExpressionFeatureAbstract<T>, IDisableExpressionFeature
        where T : IAspect
    {
        public DisableExpressionFeature(Expression<Func<T, bool>> expression) : base(expression)
        {
        }
    }
}