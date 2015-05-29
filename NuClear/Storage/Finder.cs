using System;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Core;
using NuClear.Storage.Futures;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace NuClear.Storage
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

        public FutureSequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readableDomainContextProvider.Get().GetQueryableSource<TSource>();
            return new QueryableFutureSequence<TSource>(queryableSource.Where(findSpecification.Predicate));
        }
    }
}
