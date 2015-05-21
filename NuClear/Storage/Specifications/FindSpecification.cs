using System;
using System.Linq.Expressions;

namespace NuClear.Storage.Specifications
{
    /// <summary>
    /// Specification pattern base type.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class FindSpecification<TEntity> where TEntity : class
    {
        private readonly Expression<Func<TEntity, bool>> _predicate;

        public FindSpecification(Expression<Func<TEntity, bool>> predicate)
        {
            _predicate = predicate;
        }

        internal Expression<Func<TEntity, bool>> Predicate
        {
            get { return _predicate; }
        }

        public static FindSpecification<TEntity> operator |(FindSpecification<TEntity> spec1, FindSpecification<TEntity> spec2)
        {
            return new FindSpecification<TEntity>(spec1.Predicate.Or(spec2.Predicate));
        }

        public static FindSpecification<TEntity> operator &(FindSpecification<TEntity> spec1, FindSpecification<TEntity> spec2)
        {
            return new FindSpecification<TEntity>(spec1.Predicate.And(spec2.Predicate));
        }

        public static FindSpecification<TEntity> operator !(FindSpecification<TEntity> spec)
        {
            return new FindSpecification<TEntity>(spec.Predicate.Not());
        }

        public static bool operator true(FindSpecification<TEntity> fs)
        {
            return false;
        }

        public static bool operator false(FindSpecification<TEntity> fs)
        {
            return false;
        }
    }
}