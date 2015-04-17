using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions
{
    public static class Condition
    {
        public static LambdaExpressionsSequenceBuilder On<TAspect>(Expression<Func<TAspect, bool>> expression)
            where TAspect : IAspect
        {
            return new LambdaExpressionsSequenceBuilder(expression);
        }
    }
}
