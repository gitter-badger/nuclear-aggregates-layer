using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface ISelectSpecification<TInput, TOutput>
    {
        Expression<Func<TInput, TOutput>> Selector { get; }
    }
}
