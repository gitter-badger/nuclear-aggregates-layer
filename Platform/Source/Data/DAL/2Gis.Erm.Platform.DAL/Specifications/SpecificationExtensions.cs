using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL.Specifications
{
    public static class SpecificationExtensions
    {
        private const string IdPropertyName = "Id";
        
        public static IQueryable<T> Find<T>(this IQueryable<T> queryable, IFindSpecification<T> findSpecification)
        {
            return queryable.Where(findSpecification.Predicate);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, IFindSpecification<T> specification)
        {
            return source.Where(specification.Predicate);
        }
        
        public static long ExtractEntityId<TEntity>(this IFindSpecification<TEntity> findSpecification)
        {
            var predicate = findSpecification.Predicate;

            var entityType = GetEntityType(predicate.Type);

            if (!typeof(IEntityKey).IsAssignableFrom(entityType))
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

        public static long[] ExtractEntityIds<TEntity>(this IFindSpecification<TEntity> findSpecification)
        {
            var predicate = findSpecification.Predicate;

            var entityType = GetEntityType(predicate.Type);

            if (!typeof(IEntityKey).IsAssignableFrom(entityType))
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
            if (!typeof(IEnumerable<long>).IsAssignableFrom(idCollection.Type))
            {
                throw new ArgumentException(string.Format("Expected that expression {0} is assignable to IEnumerable<long>", idCollection), "findSpecification");
            }

            return Expression.Lambda<Func<IEnumerable<long>>>(idCollection).Compile().Invoke().ToArray();
        }

        private static bool IsPropertyIdAccess(Expression expression)
        {
            var member = expression as MemberExpression;

            return member != null &&
                   typeof(IEntityKey).IsAssignableFrom(member.Member.DeclaringType) &&
                   member.Member.Name == IdPropertyName;
        }

        private static Type GetEntityType(Type func)
        {
            if (func.GetGenericTypeDefinition() != typeof(Func<,>))
            {
                return null;
            }

            var arguments = func.GetGenericArguments();
            if (arguments[1] != typeof(bool))
            {
                return null;
            }

            return arguments[0];
        }
    }
}
