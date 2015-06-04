using System;
using System.Linq;

using NuClear.Storage.Core;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

namespace NuClear.Storage.Readings
{
    public class Finder : IFinder
    {
        private readonly IReadableDomainContextProvider _readableDomainContextProvider;

        public Finder(IReadableDomainContextProvider readableDomainContextProvider)
        {
            if (readableDomainContextProvider == null)
            {
                throw new ArgumentNullException("readableDomainContextProvider");
            }

            _readableDomainContextProvider = readableDomainContextProvider;
        }

        public Sequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readableDomainContextProvider.Get().GetQueryableSource<TSource>();
            return new QueryableSequence<TSource>(queryableSource.Where(findSpecification.Predicate));
        }
    }
}
