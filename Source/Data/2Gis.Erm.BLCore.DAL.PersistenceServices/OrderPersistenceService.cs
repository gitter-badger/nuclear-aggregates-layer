using System;
using System.Transactions;

using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    // TODO {all, 05.08.2013}: Вопрос корректна ли данная реализация - т.к. она обеспечивает генерацию уникальных значений только в рамках одного persistance, пока 1 инсталяция 1 persistance все вроде бы хорошо, однако, большой вопрос по пригодности к scaleout такого подхода
    public sealed class OrderPersistenceService : IOrderPersistenceService
    {
        private const string GenerateIndexProcedureName = "Billing.GenerateIndex";
        private const string GenerateIndexParameterName = "IndexCode";
        private const string GenerateIndexOrderGlobalIndex = "OrderGlobalIndex";

        private readonly IDatabaseCaller _databaseCaller;

        public OrderPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public long GenerateNextOrderUniqueNumber()
        {
            // Генерация идентификатора происходит вне внешней транзакций, поскольку:
            // 1. нам требуется, чтобы блокировка на соответвующий объект существовала как можно меньше.
            // 2. нам не требуется, чтобы при откате внешней транзакции счётчик возвращался к прошлому значению.
            using (var transaction = new TransactionScope(TransactionScopeOption.Suppress, DefaultTransactionOptions.Default))
            {
                var objectResult =
                    _databaseCaller.ExecuteProcedureWithReturnValue<long?>(GenerateIndexProcedureName,
                                                                           new Tuple<string, object>(GenerateIndexParameterName, GenerateIndexOrderGlobalIndex));
                transaction.Complete();
            return objectResult.HasValue ? objectResult.Value : 0L;
        }
    }
}
}