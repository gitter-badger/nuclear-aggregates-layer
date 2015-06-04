using NuClear.Storage.Specifications;

namespace NuClear.Storage.Readings
{
    /// <summary>
    /// Readonly data access contract
    /// </summary>
    public interface IFinder 
    {
        /// <summary>
        /// Compose sequence based on findSpecification.
        /// </summary>
        Sequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class;
    }
}
