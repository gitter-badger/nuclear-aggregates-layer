using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public sealed class HideExpressionsFeature : IUIElementLambdaExpressionsSequenceFeature
    {
        public HideExpressionsFeature(LogicalOperation logicalOperation, LambdaExpression[] expressions)
        {
            Expressions = expressions;
            LogicalOperation = logicalOperation;
        }

        public LogicalOperation LogicalOperation { get; private set; }
        public LambdaExpression[] Expressions { get; private set; }
    }
}