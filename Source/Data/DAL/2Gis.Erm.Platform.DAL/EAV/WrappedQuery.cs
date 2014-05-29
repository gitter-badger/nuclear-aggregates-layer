using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class WrappedQuery : IQueryable, IOrderedQueryable
    {
        private readonly IQueryable _queryable;
        private readonly IQueryProvider _provider;

        public WrappedQuery(IQueryable queryable, IQueryProvider provider)
        {
            _queryable = queryable;
            _provider = provider;
        }

        public Expression Expression
        {
            get { return _queryable.Expression; }
        }

        public Type ElementType
        {
            get { return _queryable.ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return _provider; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        public IQueryable Unwrap()
        {
            return _queryable;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class WrappedQuery<T> : WrappedQuery, IQueryable<T>, IOrderedQueryable<T>
    {
        private readonly IQueryable<T> _queryable;

        public WrappedQuery(IQueryable<T> queryable, IQueryProvider provider)
            : base(queryable, provider)
        {
            _queryable = queryable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }
    }
}