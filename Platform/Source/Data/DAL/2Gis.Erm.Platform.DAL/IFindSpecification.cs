using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.DAL
{
    // FIXME {d.ivanov, 15.09.2014}: Move to 2Gis.Erm.Platform.Common
    /// <summary>
    /// Inteface for findSpecification pattern.
    /// </summary>
    public interface IFindSpecification<TEntity>
    {
        /// <summary>
        /// The criteria of the findSpecification.
        /// </summary>
        Expression<Func<TEntity, bool>> Predicate { get; }

        /// <summary>
        /// Return true/false whethe the Entity object meet the criteria encapsulated by the
        /// findSpecification.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsSatisfiedBy(TEntity entity);
    }
}