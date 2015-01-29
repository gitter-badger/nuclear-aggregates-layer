using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public class LambdaExpressionsSequenceBuilder : ILambdaExpressionsSequenceWithAndOperationBuilder, ILambdaExpressionsSequenceWithOrOperationBuilder
    {
        public LambdaExpressionsSequenceBuilder(params LambdaExpression[] expressions)
        {
            Expressions = new List<LambdaExpression>(expressions);
        }

        public IEnumerable<LambdaExpression> Expressions { get; private set; }

        public LogicalOperation SequenceOperation { get; private set; }

        public static implicit operator LambdaExpressionsSequence(LambdaExpressionsSequenceBuilder builder)
        {
            return builder.ToSequence();
        }

        public LambdaExpressionsSequence ToSequence()
        {
            return new LambdaExpressionsSequence(null, LogicalOperation.And);
        }

        public ILambdaExpressionsSequenceWithAndOperationBuilder And(ILambdaExpressionsSequenceWithAndOperationBuilder builder2)
        {
            SequenceOperation = LogicalOperation.And;
            Expressions = Expressions.Concat(builder2.Expressions);
            return this;
        }

        public ILambdaExpressionsSequenceWithOrOperationBuilder Or(ILambdaExpressionsSequenceWithOrOperationBuilder builder2)
        {
            SequenceOperation = LogicalOperation.Or;
            Expressions = Expressions.Concat(builder2.Expressions);
            return this;
        }
    }
}