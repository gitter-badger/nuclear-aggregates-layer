using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.AdoNet
{
    public sealed class StoredProcedureCallException : Exception
    {
        public StoredProcedureCallException(Exception inner, string procedureName, params Tuple<string, object>[] procedureArguments)
            : base(GenerateMessage(inner, procedureName, procedureArguments), inner)
        {
        }

        public StoredProcedureCallException(Exception inner, string procedureName, IEnumerable<Tuple<string, object>> procedureArguments)
            : base(GenerateMessage(inner, procedureName, procedureArguments), inner)
        {
        }

        private static string GenerateMessage(Exception inner, string procedureName, IEnumerable<Tuple<string, object>> procedureArguments)
        {
            var items = procedureArguments.Select(tuple => string.Format("{0}: '{1}'", tuple.Item1, tuple.Item2));
            return string.Format("Ошибка при выполнении хранимой процедуры '{0}'. Сообщение: {1}. Aргументы вызова: {2}",
                                 procedureName,
                                 inner.Message,
                                 string.Join(", ", items));
        }
    }
}
