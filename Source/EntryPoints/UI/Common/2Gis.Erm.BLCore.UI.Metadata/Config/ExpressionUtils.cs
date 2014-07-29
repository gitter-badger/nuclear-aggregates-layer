using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config
{
    public static class ExpressionUtils
    {
        public static string AsDynamicQueryableExpression<TEntity>(this Expression<Func<TEntity, object>> selectExpression)
        {
            return "it." + StaticReflection.GetFullPropertyName(selectExpression);
        }
    }
}