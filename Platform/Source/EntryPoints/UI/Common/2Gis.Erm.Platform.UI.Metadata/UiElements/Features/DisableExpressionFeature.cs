using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features
{
    // тут, наверное, наследование к месту будет
    public sealed class DisableExpressionFeature<T> : IDisableExpressionFeature
        where T : IViewModelAbstract
    {
        public DisableExpressionFeature(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
        }

        public Expression<Func<T, bool>> Expression { get; private set; }

        public bool TryExecute(IViewModelAbstract viewModel, out bool result)
        {
            result = false;
            if (!(viewModel is T))
            {
                return false;
            }

            var func = Expression.Compile();
            result = func.Invoke((T)viewModel);
            return true;
        }
    }
}