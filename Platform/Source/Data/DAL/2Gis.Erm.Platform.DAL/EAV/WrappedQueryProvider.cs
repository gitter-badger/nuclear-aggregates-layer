using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal sealed class WrappedQueryProvider : System.Linq.IQueryProvider
    {
        private readonly System.Linq.IQueryProvider _provider;

        public WrappedQueryProvider(System.Linq.IQueryProvider provider)
        {
            _provider = provider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var queryable = _provider.CreateQuery(expression);
            var provider = new WrappedQueryProvider(queryable.Provider);

            if (!queryable.GetType().IsGenericType)
            {
                return new WrappedQuery(queryable, provider);
            }

            var genericType = typeof(WrappedQuery<>).MakeGenericType(queryable.GetType().GetGenericArguments());
            return (IQueryable)Activator.CreateInstance(genericType, new object[] { queryable, provider });
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var queryable = _provider.CreateQuery<TElement>(expression);
            var provider = new WrappedQueryProvider(queryable.Provider);
            return new WrappedQuery<TElement>(queryable, provider);
        }

        public object Execute(Expression expression)
        {
            expression.Check();
            expression = expression.Unwrap();
            return _provider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            expression.Check();
            expression = expression.Unwrap();
            return _provider.Execute<TResult>(expression);
        }
    }
}