﻿using System;
using System.Transactions;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Aggregates;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public sealed class AggregateServiceIsolator : IAggregateServiceIsolator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonLog _logger;

        public AggregateServiceIsolator(IUnitOfWork unitOfWork, ICommonLog logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public void Execute<TAggregateService>(Action<TAggregateService> action) where TAggregateService : class, IAggregateRepository
        {
            Func<TAggregateService, bool> ignoreResultFunc = x =>
            {
                action(x);
                return true;
            };

            Execute(ignoreResultFunc);
        }

        public TResult Execute<TAggregateService, TResult>(Func<TAggregateService, TResult> func) where TAggregateService : class, IAggregateRepository
        {
            try
            {
                // FIXME {all, 15.01.2014}: добавить поддержку поведения без использования deferred save - т.е. uowScope будет использоваться только для управления временем жизни domaincontext
                using (var uowScope = _unitOfWork.CreateScope())
                {
                    var aggregateService = uowScope.CreateRepository<TAggregateService>();
                    var result = func(aggregateService);

                    uowScope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Isolated aggregate service execution failed");
                throw;
            }
        }

        public void TransactedExecute<TAggregateService>(TransactionScopeOption transactionScopeOption, Action<TAggregateService> action) where TAggregateService : class, IAggregateRepository
        {
            using (var transaction = new TransactionScope(transactionScopeOption, DefaultTransactionOptions.Default))
            {
                Execute(action);
                transaction.Complete();
            }
        }

        public TResult TransactedExecute<TAggregateService, TResult>(TransactionScopeOption transactionScopeOption, Func<TAggregateService, TResult> func) where TAggregateService : class, IAggregateRepository
        {
            using (var transaction = new TransactionScope(transactionScopeOption, DefaultTransactionOptions.Default))
            {
                var result = Execute(func);
                transaction.Complete();

                return result;
            }
        }
    }
}