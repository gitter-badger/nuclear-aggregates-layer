using System.Collections.Generic;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public interface ILambdaExpressionsSequenceBuilder
    {
        IEnumerable<LambdaExpression> Expressions { get; }
        LogicalOperation SequenceOperation { get; }
        LambdaExpressionsSequence ToSequence();
    }
}
