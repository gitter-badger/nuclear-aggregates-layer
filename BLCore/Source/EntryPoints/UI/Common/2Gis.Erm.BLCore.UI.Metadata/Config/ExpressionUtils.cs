using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config
{
    public static class ExpressionUtils
    {
        public static string AsDynamicQueryableExpression<TEntity>(this Expression<Func<TEntity, object>> selectExpression)
        {
            return "it." + StaticReflection.GetFullPropertyName(selectExpression);
        }

        public static bool TryExecuteAspectLambda<TReturn>(this LambdaExpression expression, IAspect aspectHost, out TReturn result)
        {
            result = default(TReturn);

            if (!expression.ReturnType.IsAssignableFrom(typeof(TReturn)))
            {
                return false;
            }

            if (expression.Parameters.Count() != 1)
            {
                return false;
            }

            var lambdaParameter = expression.Parameters[0];
           
            if (!lambdaParameter.Type.IsInstanceOfType(aspectHost))
            {
                return false;
            }

            try
            {
                result = (TReturn)expression.Compile().DynamicInvoke(aspectHost);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}