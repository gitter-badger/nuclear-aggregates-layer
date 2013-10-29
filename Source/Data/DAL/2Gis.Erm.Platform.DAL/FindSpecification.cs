using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Specification pattern base type.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class FindSpecification<TEntity> : IFindSpecification<TEntity> where TEntity : class
    {
        private readonly Expression<Func<TEntity, bool>> _predicate;

        public FindSpecification(Expression<Func<TEntity, bool>> predicate)
        {
            _predicate = predicate;
        }

        public Expression<Func<TEntity, bool>> Predicate
        {
            get { return _predicate; }
        }

        public static FindSpecification<TEntity> operator |(FindSpecification<TEntity> x, FindSpecification<TEntity> y)
        {
            return x.Or(y);
        }

        public static FindSpecification<TEntity> operator &(FindSpecification<TEntity> x, FindSpecification<TEntity> y)
        {
            return x.And(y);
        }

        public static FindSpecification<TEntity> operator !(FindSpecification<TEntity> x)
        {
            return x.Not();
        }

        public static bool operator true(FindSpecification<TEntity> fs)
        {
            return false;
        }

        public static bool operator false(FindSpecification<TEntity> fs)
        {
            return false;
        }

        public bool IsSatisfiedBy(TEntity entity)
        {
            return _predicate.Compile().Invoke(entity);
        }
    }
}