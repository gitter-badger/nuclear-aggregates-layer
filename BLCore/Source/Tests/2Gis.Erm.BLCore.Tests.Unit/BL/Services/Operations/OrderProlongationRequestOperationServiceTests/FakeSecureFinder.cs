using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests
{
    class FakeSecureFinder : ISecureFinder
    {
        public FakeSecureFinder()
        {
            Storage = new List<object>();
        }

        public List<object> Storage { get; private set; }

        public Sequence<TEntity> Find<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class
        {
            return new QueryableSequence<TEntity>(Storage.OfType<TEntity>().AsQueryable().Where(findSpecification).AsQueryable());
        }
    }
}
