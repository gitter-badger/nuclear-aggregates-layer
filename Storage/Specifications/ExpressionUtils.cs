using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NuClear.Storage.Specifications
{
    public static class ExpressionUtils
    {
        /// <summary>
        /// Combines two expression using "or"
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            return Compose(expr1, expr2, Expression.OrElse);
        }

        /// <summary>
        /// Combines two expression using "and"
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            return Compose(expr1, expr2, Expression.AndAlso);
        }

        /// <summary>
        /// Negates the expression
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
        {
            var negated = Expression.Not(expr.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expr.Parameters);
        }

        /// <summary>
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        private static Expression<T> Compose<T>(
            Expression<T> first, 
            Expression<T> second, 
            Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
                .Zip(second.Parameters, (f, s) => new { f, s })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> _map;
            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                _map = map;
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;
                if (_map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }

                return base.VisitParameter(p);
            }
        }
    }
}
