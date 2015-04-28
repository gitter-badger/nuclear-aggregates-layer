using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public class LambdaExpressionsSequenceBuilder 
    {
        public LambdaExpressionsSequenceBuilder(params LambdaExpression[] expressions)
        {
            Expressions = new List<LambdaExpression>(expressions);
        }

        public IEnumerable<LambdaExpression> Expressions { get; private set; }

        public LogicalOperation? SequenceOperation { get; private set; }

        public static implicit operator LambdaExpressionsSequence(LambdaExpressionsSequenceBuilder builder)
        {
            return builder.ToSequence();
        }

        public static LambdaExpressionsSequenceBuilder operator &(LambdaExpressionsSequenceBuilder builder1, LambdaExpressionsSequenceBuilder builder2)
        {
            return builder1.And(builder2);
        }

        public static LambdaExpressionsSequenceBuilder operator |(LambdaExpressionsSequenceBuilder builder1, LambdaExpressionsSequenceBuilder builder2)
        {
            return builder1.Or(builder2);
        }

        public LambdaExpressionsSequence ToSequence()
        {
            if (!SequenceOperation.HasValue)
            {
                throw new InvalidOperationException("The sequence must have specified logical operator");
            }

            return new LambdaExpressionsSequence(Expressions.ToArray(), SequenceOperation.Value);
        }

        public LambdaExpressionsSequenceBuilder And(LambdaExpressionsSequenceBuilder builder2)
        {
            if (SequenceOperation.HasValue && SequenceOperation.Value != LogicalOperation.And)
            {
                throw new InvalidOperationException("The sequence can't contain different operators");
            }

            SequenceOperation = LogicalOperation.And;
            Expressions = Expressions.Concat(builder2.Expressions);
            return this;
        }

        public LambdaExpressionsSequenceBuilder Or(LambdaExpressionsSequenceBuilder builder2)
        {
            if (SequenceOperation.HasValue && SequenceOperation.Value != LogicalOperation.Or)
            {
                throw new InvalidOperationException("The sequence can't contain different operators");
            }

            SequenceOperation = LogicalOperation.Or;
            Expressions = Expressions.Concat(builder2.Expressions);
            return this;
        }
    }
}