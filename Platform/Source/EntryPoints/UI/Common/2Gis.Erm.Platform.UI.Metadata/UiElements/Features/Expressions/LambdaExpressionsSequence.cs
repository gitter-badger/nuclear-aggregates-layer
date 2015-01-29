using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public sealed class LambdaExpressionsSequence : ILambdaExpressionsSequence
    {
        public LambdaExpressionsSequence(LambdaExpression[] expressions, LogicalOperation expressionsCombination)
        {
            ExpressionsCombination = expressionsCombination;
            Expressions = expressions;
        }

        public LogicalOperation ExpressionsCombination { get; private set; }
        public LambdaExpression[] Expressions { get; private set; }
    }
}
