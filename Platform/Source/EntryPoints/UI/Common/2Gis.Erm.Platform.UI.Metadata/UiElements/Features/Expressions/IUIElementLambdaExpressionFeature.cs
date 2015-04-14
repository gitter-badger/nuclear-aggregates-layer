using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public interface IUIElementLambdaExpressionFeature : IUIElementFeature
    {
        LambdaExpression Expression { get; }
    }
}
