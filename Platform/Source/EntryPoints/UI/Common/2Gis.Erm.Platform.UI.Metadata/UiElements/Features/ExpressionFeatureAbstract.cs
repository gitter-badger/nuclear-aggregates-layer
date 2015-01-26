using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    public abstract class ExpressionFeatureAbstract<T> : IUIElementExpressionFeature
        where T : IAspect
    {
        protected ExpressionFeatureAbstract(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
        }

        public Expression<Func<T, bool>> Expression { get; private set; }

        public bool TryExecute(IAspect aspect, out bool result)
        {
            result = false;
            if (!(aspect is T))
            {
                return false;
            }

            var func = Expression.Compile();
            result = func.Invoke((T)aspect);
            return true;
        }
    }
}