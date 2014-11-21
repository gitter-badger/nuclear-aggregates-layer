using System;

using DoubleGis.Erm.Platform.DAL.AdoNet;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    // TODO {all, 05.08.2013}: Вопрос корректна ли данная реализация - т.к. она обеспечивает генерацию уникальных значений только в рамках одного persistance, пока 1 инсталяция 1 persistance все вроде бы хорошо, однако, большой вопрос по пригодности к scaleout такого подхода
    public sealed class BargainPersistenceService : IBargainPersistenceService
    {
        private const string GenerateIndexProcedureName = "Billing.GenerateIndex";
        private const string GenerateIndexParameterName = "IndexCode";
        private const string GenerateIndexBargainGlobalIndex = "BargainGlobalIndex";

        private readonly IDatabaseCaller _databaseCaller;

        public BargainPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public long GenerateNextBargainUniqueNumber()
        {
            var objectResult = _databaseCaller.ExecuteProcedureWithResultSingleValue<long?>(GenerateIndexProcedureName,
                              new Tuple<string, object>(GenerateIndexParameterName,
                                                        GenerateIndexBargainGlobalIndex));
            return objectResult.HasValue ? objectResult.Value : 0L;
        }
    }
}