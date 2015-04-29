using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NuClear.Storage.Specifications
{
    public static class ExpressionUtils
    {
        /// <summary>
        /// Combines two expression using "or"
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
            where T : class
        {
            return Compose(expr1, expr2, Expression.OrElse);
        }

        /// <summary>
        /// Combines two expression using "and"
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
            where T : class
        {
            return Compose(expr1, expr2, Expression.AndAlso);
        }

        /// <summary>
        /// Negates the expression
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
            where T : class
        {
            var negated = Expression.Not(expr.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expr.Parameters);
        }

        /// <summary>
        /// Replaces type of argument of expression.
        /// </summary>
        public static Expression<Func<T2, bool>> ReplaceParameterType<T1, T2>(this Expression<Func<T1, bool>> expression)
            where T1 : class
            where T2 : class
        {
            var rebinder = new ParameterTypeRebinder<T1, T2>();
            return rebinder.ReplaceParameters(expression);
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

        private class ParameterTypeRebinder<T1, T2> : ExpressionVisitor
        {
            private readonly Dictionary<string, ParameterExpression> _parametersMapping = new Dictionary<string, ParameterExpression>();

            public Expression<Func<T2, bool>> ReplaceParameters(Expression<Func<T1, bool>> exp)
            {
                var newParameters = exp.Parameters.Select(ConvertParameterExpression).ToArray();

                return Expression.Lambda<Func<T2, bool>>(Visit(exp.Body), newParameters);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                var propertyInfo = node.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    if (propertyInfo.PropertyType == typeof(T1))
                    {
                        var parameterExpression = (ParameterExpression)node.Expression;
                        var replacementParameterExpression = _parametersMapping[parameterExpression.Name];

                        var replacementPropery = typeof(T2).GetRuntimeProperty(node.Member.Name);
                        var newNode = Expression.MakeMemberAccess(replacementParameterExpression, replacementPropery);
                        return base.VisitMember(newNode);
                    }

                    if (node.Expression is ParameterExpression)
                    {
                        // Если стучимся к свойству через интерфейс (например, IEntityKey.Id)
                        var parameterExpression = (ParameterExpression)node.Expression;
                        ParameterExpression replacementParameterExpression;
                        if (_parametersMapping.TryGetValue(parameterExpression.Name, out replacementParameterExpression))
                        {
                            // Свойство ищем на самом классе - для явно реализованных интерфейсов может и сломаться.
                            var replacementPropery = typeof(T2).GetRuntimeProperty(node.Member.Name);
                            var newNode = Expression.MakeMemberAccess(replacementParameterExpression, replacementPropery);
                            return base.VisitMember(newNode);
                        }
                    }
                }

                return base.VisitMember(node);
            }

            private ParameterExpression ConvertParameterExpression(ParameterExpression parameterExpression)
            {
                if (parameterExpression.Type == typeof(T1))
                {
                    var newParameter = Expression.Parameter(typeof(T2), parameterExpression.Name);
                    _parametersMapping[parameterExpression.Name] = newParameter;
                    return newParameter;
                }

                return parameterExpression;
            }
        }
    }
}
