using System;
using System.Linq.Expressions;

namespace NuClear.Storage.Specifications
{
    public interface ISelectSpecification<TInput, TOutput>
    {
        Expression<Func<TInput, TOutput>> Selector { get; }
    }
}
