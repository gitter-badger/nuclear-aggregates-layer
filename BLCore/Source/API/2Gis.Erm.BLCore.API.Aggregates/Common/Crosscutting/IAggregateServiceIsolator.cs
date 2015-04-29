using System;
using System.Transactions;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting
{
    public interface IAggregateServiceIsolator
    {
        void Execute<TAggregateService>(Action<TAggregateService> action)
            where TAggregateService : class, IAggregateRepository;

        TResult Execute<TAggregateService, TResult>(Func<TAggregateService, TResult> func)
            where TAggregateService : class, IAggregateRepository;

        void TransactedExecute<TAggregateService>(TransactionScopeOption transactionScopeOption, Action<TAggregateService> action)
            where TAggregateService : class, IAggregateRepository;

        TResult TransactedExecute<TAggregateService, TResult>(TransactionScopeOption transactionScopeOption, Func<TAggregateService, TResult> func)
            where TAggregateService : class, IAggregateRepository;
    }
}