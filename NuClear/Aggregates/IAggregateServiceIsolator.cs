using System;

namespace NuClear.Aggregates
{
    public interface IAggregateServiceIsolator
    {
        void Execute<TAggregateService>(Action<TAggregateService> action)
            where TAggregateService : class, IAggregateService;

        TResult Execute<TAggregateService, TResult>(Func<TAggregateService, TResult> func)
            where TAggregateService : class, IAggregateService;
    }
}