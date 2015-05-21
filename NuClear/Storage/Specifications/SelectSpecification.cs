using System;
using System.Linq.Expressions;

namespace NuClear.Storage.Specifications
{
    public class SelectSpecification<TInput, TOutput>
    {
        private readonly Expression<Func<TInput, TOutput>> _selector;

        public SelectSpecification(Expression<Func<TInput, TOutput>> selector)
        {
            _selector = selector;
        }

        internal Expression<Func<TInput, TOutput>> Selector
        {
            get { return _selector; }
        }
    }
}
