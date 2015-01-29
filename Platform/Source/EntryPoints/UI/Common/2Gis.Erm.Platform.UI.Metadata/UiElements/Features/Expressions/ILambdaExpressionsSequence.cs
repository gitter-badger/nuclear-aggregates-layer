using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public interface ILambdaExpressionsSequence
    {
        LogicalOperation ExpressionsCombination { get; }
        LambdaExpression[] Expressions { get; }
    }
}
