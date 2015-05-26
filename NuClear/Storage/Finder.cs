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
        private readonly IReadDomainContextProvider _readDomainContextProvider;

        public Finder(IReadDomainContextProvider readDomainContextProvider)
        {
            if (readDomainContextProvider == null)
            {
                throw new ArgumentNullException("readDomainContextProvider");
            }

            _readDomainContextProvider = readDomainContextProvider;
        }

        public FutureSequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TSource>();
            return new QueryableFutureSequence<TSource>(queryableSource.Where(findSpecification.Predicate));
        }
    }
}
