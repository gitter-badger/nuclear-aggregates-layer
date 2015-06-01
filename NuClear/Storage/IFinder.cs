using NuClear.Storage.Futures;
using NuClear.Storage.Specifications;

namespace NuClear.Storage
{
    /// <summary>
    /// Интерфейс для readonly доступа к данным
    /// </summary>
    public interface IFinder 
    {
        /// <summary>
        /// Compose future sequence based on findSpecification.
        /// </summary>
        Sequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class;
    }
}
