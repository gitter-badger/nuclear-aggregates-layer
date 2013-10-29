using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.Specifications
{
    public static class SpecificationExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> source, IFindSpecification<T> specification)
        {
            return source.Where(specification.Predicate);
        }
    }
}
