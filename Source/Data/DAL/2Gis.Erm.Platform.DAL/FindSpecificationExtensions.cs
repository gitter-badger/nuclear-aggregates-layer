using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Extensions to combine FindSpecifications
    /// </summary>
    public static class FindSpecificationExtensions
    {
        /// <summary>
        /// Replaces type of argument of specifications.
        /// </summary>
        public static FindSpecification<T2> ReplaceParameterType<T1, T2>(this IFindSpecification<T1> spec)
            where T1 : class
            where T2 : class
        {
            ParameterTypeRebinder<T1, T2> rebinder = new ParameterTypeRebinder<T1, T2>();
            return new FindSpecification<T2>(rebinder.ReplaceParameters(spec.Predicate));
        }

        /// <summary>
        /// Combines two find specifications using "or"
        /// </summary>
        internal static FindSpecification<TEntity> Or<TEntity>(this IFindSpecification<TEntity> spec1,
                                                              IFindSpecification<TEntity> spec2) where TEntity : class
        {
            return new FindSpecification<TEntity>(Compose(spec1.Predicate, spec2.Predicate, Expression.OrElse));
        }

        /// <summary>
        /// Combines two find specifications using "and"
        /// </summary>
        internal static FindSpecification<TEntity> And<TEntity>(this IFindSpecification<TEntity> spec1,
                                                               IFindSpecification<TEntity> spec2) where TEntity : class
        {
            return new FindSpecification<TEntity>(Compose(spec1.Predicate, spec2.Predicate, Expression.AndAlso));
        }

        /// <summary>
        /// Negates the find specification
        /// </summary>
        internal static FindSpecification<TEntity> Not<TEntity>(this IFindSpecification<TEntity> spec) where TEntity : class
        {
            var negated = Expression.Not(spec.Predicate.Body);
            return new FindSpecification<TEntity>(Expression.Lambda<Func<TEntity, bool>>(negated, spec.Predicate.Parameters));
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

            protected override Expression VisitMember(MemberExpression node)
            {
                if ((node.Member.MemberType & MemberTypes.Property) == MemberTypes.Property)
                {
                    if (node.Member.ReflectedType == typeof(T1))
                    {
                        var parameterExpression = (ParameterExpression)node.Expression;
                        var replacementParameterExpression = _parametersMapping[parameterExpression.Name];

                        var replacementPropery = typeof(T2).GetProperty(node.Member.Name);
                        var newNode = Expression.MakeMemberAccess(replacementParameterExpression, replacementPropery);
                        return base.VisitMember(newNode);
                    }
                    else if (node.Expression is ParameterExpression)
                    {
                        // Если стучимся к свойству через интерфейс (например, IEntityKey.Id)
                        var parameterExpression = (ParameterExpression)node.Expression;
                        ParameterExpression replacementParameterExpression;
                        if (_parametersMapping.TryGetValue(parameterExpression.Name, out replacementParameterExpression))
                        {
                            // Свойство ищем на самом классе - для явно реализованных интерфейсов может и сломаться.
                            var replacementPropery = typeof(T2).GetProperty(node.Member.Name);
                            var newNode = Expression.MakeMemberAccess(replacementParameterExpression, replacementPropery);
                            return base.VisitMember(newNode);
                        }
                    }
                }
                return base.VisitMember(node);
            }
        }
    }
}