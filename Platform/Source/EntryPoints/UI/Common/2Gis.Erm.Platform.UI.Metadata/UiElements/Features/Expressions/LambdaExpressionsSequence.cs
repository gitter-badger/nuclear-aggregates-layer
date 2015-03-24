using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public sealed class LambdaExpressionsSequence : ILambdaExpressionsSequence
    {
        public LambdaExpressionsSequence(LambdaExpression[] expressions, LogicalOperation expressionsCombination)
        {
            LogicalOperation = expressionsCombination;
            Expressions = expressions;
        }

        public LogicalOperation LogicalOperation { get; private set; }
        public LambdaExpression[] Expressions { get; private set; }
    }
}
