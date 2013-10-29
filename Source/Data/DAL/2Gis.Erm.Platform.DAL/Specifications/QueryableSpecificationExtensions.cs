using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.Specifications
{
    public static class QueryableSpecificationExtensions
    {
        public static IQueryable<T> Find<T>(this IQueryable<T> queryable, IFindSpecification<T> findSpecification)
        {
            return queryable.Where(findSpecification.Predicate);
        }
    }
}