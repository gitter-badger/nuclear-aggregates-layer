using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NuClear.Model.Common.Entities.Aspects;

namespace NuClear.Storage.Specifications
{
    public static class SpecificationExtensions
    {
        private const string IdPropertyName = "Id";
        
        public static IQueryable<T> Find<T>(this IQueryable<T> queryable, FindSpecification<T> findSpecification) where T : class
        {
            return queryable.Where(findSpecification.Predicate);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, FindSpecification<T> specification) where T : class
        {
            return source.Where(specification.Predicate);
        }

        public static IQueryable<T2> Select<T1, T2>(this IQueryable<T1> source, SelectSpecification<T1, T2> specification) where T1 : class
        {
            return source.Select(specification.Selector);
        }
        
        public static long ExtractEntityId<TEntity>(this FindSpecification<TEntity> findSpecification) where TEntity : class
        {
            var predicate = findSpecification.Predicate;

            var entityType = GetEntityType(predicate.Type);

            if (!typeof(IEntityKey).GetTypeInfo().IsAssignableFrom(entityType.GetTypeInfo()))
            {
                throw new ArgumentException(string.Format("Expected that {0} is IEntityKey", entityType.Name), "findSpecification");
            }

            if (predicate.Body.NodeType != ExpressionType.Equal)
            {
                throw new ArgumentException(string.Format("Expected that expression {0} contains single equality", predicate.Body), "findSpecification");
            }

            var equality = (BinaryExpression)predicate.Body;
            var idProperty = new[] { equality.Left, equality.Right }.Where(IsPropertyIdAccess).ToArray();
            if (idProperty.Length != 1)
            {
                throw new ArgumentException("Exprected one and only one Id property access", "findSpecification");
            }

            var idExpression = new[] { equality.Left, equality.Right }.Except(idProperty).Single();
            if (idExpression.Type != typeof(long))
            {
                throw new ArgumentException(string.Format("Expected that expression {0} is of type long", idExpression), "findSpecification");
            }

            return Expression.Lambda<Func<long>>(idExpression).Compile().Invoke();
        }

        public static long[] ExtractEntityIds<TEntity>(this FindSpecification<TEntity> findSpecification) where TEntity : class
        {
            var predicate = findSpecification.Predicate;

            var entityType = GetEntityType(predicate.Type);

            if (!typeof(IEntityKey).GetTypeInfo().IsAssignableFrom(entityType.GetTypeInfo()))
            {
                throw new ArgumentException(string.Format("Expected that {0} is IEntityKey", entityType.Name), "findSpecification");
            }

            if (predicate.Body.NodeType != ExpressionType.Call)
            {
                throw new ArgumentException(string.Format("Expected that expression {0} contains single method call", predicate.Body), "findSpecification");
            }

            var callToContains = (MethodCallExpression)predicate.Body;
            if (callToContains.Method.Name != "Contains")
            {
                throw new ArgumentException(string.Format("Expected that expression {0} is call of 'Contains' method", predicate.Body), "findSpecification");
            }

            var idCollection = callToContains.Arguments[0];
            if (!typeof(IEnumerable<long>).GetTypeInfo().IsAssignableFrom(idCollection.Type.GetTypeInfo()))
            {
                throw new ArgumentException(string.Format("Expected that expression {0} is assignable to IEnumerable<long>", idCollection), "findSpecification");
            }

            return Expression.Lambda<Func<IEnumerable<long>>>(idCollection).Compile().Invoke().ToArray();
        }

        public static FindSpecification<T2> ReplaceType<T1, T2>(this FindSpecification<T1> expression)
            where T1 : class
            where T2 : class
        {
            var rebinder = new ParameterTypeRebinder<T1, T2>();
            return new FindSpecification<T2>(rebinder.ReplaceParameters(expression.Predicate));
        }

        private static bool IsPropertyIdAccess(Expression expression)
        {
            var member = expression as MemberExpression;

            return member != null &&
                   typeof(IEntityKey).GetTypeInfo().IsAssignableFrom(member.Member.DeclaringType.GetTypeInfo()) &&
                   member.Member.Name == IdPropertyName;
        }

        private static Type GetEntityType(Type func)
        {
            if (func.GetGenericTypeDefinition() != typeof(Func<,>))
            {
                return null;
            }

            var arguments = func.GenericTypeArguments;
            if (arguments[1] != typeof(bool))
            {
                return null;
            }

            return arguments[0];
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
