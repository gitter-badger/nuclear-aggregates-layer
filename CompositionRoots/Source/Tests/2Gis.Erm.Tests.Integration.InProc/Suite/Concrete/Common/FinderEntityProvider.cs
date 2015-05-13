﻿using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common
{
    public sealed class FinderAppropriateEntityProvider<TEntity> : IAppropriateEntityProvider<TEntity> 
        where TEntity : class, IEntity
    {
        private readonly IFinder _finder;

        public FinderAppropriateEntityProvider(IFinder finder)
        {
            _finder = finder;
        }

        public TEntity Get(IFindSpecification<TEntity> spec)
        {
            return _finder.Find(spec).FirstOrDefault();
        }

        public IReadOnlyCollection<TEntity> Get(IFindSpecification<TEntity> spec, int maxCount)
        {
            return _finder.Find(spec).Take(maxCount).ToArray();
        }
    }
}
