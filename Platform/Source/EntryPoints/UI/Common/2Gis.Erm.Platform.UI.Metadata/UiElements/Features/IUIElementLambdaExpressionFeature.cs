using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features
{
    public interface IUIElementLambdaExpressionFeature : IUIElementFeature
    {
        LambdaExpression Expression { get; }
    }
}
