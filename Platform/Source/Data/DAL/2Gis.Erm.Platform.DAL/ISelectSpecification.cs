using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.DAL
{
    // FIXME {d.ivanov, 15.09.2014}: Move to 2Gis.Erm.Platform.Common
    public interface ISelectSpecification<TInput, TOutput>
    {
        Expression<Func<TInput, TOutput>> Selector { get; }
    }
}
