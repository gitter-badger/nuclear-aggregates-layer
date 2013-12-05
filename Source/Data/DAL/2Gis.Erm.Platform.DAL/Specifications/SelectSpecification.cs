using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.DAL.Specifications
{
    public class SelectSpecification<TInput, TOutput> : ISelectSpecification<TInput, TOutput>
    {
        private readonly Expression<Func<TInput, TOutput>> _selector;

        public SelectSpecification(Expression<Func<TInput, TOutput>> selector)
        {
            _selector = selector;
        }

        public Expression<Func<TInput, TOutput>> Selector
        {
            get { return _selector; }
        }
    }
}
