using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils.Resources;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public interface IUIElementLambdaExpressionsCombinationFeature : IUIElementFeature
    {
        ExpressionsCombination ExpressionsCombination { get; }
        LambdaExpression[] Expressions { get; }
    }
}