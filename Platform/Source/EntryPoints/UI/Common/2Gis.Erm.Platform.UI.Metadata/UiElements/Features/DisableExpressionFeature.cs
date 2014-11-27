using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features
{
    public sealed class DisableExpressionFeature : IUiElementFeature
    {
        public DisableExpressionFeature(Expression<Func<IViewModelAbstract, bool>> expression)
        {
            Expression = expression;
        }

        public Expression<Func<IViewModelAbstract, bool>> Expression { get; private set; }
    }
}