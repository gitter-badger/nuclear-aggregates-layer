using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.DAL.AdoNet
{
    public interface IDatabaseCaller
    {
        // Использовать для хранимок, в которых результат возвращается через return.
        // В этих хранимках тип возвращаемого результата ограничен числовыми типами и не может быть Null.
        T ExecuteProcedureWithReturnValue<T>(string procedureName, params Tuple<string, object>[] inputParameters);
        T ExecuteProcedureWithReturnValue<T>(string procedureName, int? commandTimeout, params Tuple<string, object>[] inputParameters);

        // Использовать для хранимок, в которых результат возвращается через select.
        T ExecuteProcedureWithSelectValue<T>(string procedureName, params Tuple<string, object>[] inputParameters);
        T ExecuteProcedureWithSelectValue<T>(string procedureName, int? commandTimeout, params Tuple<string, object>[] inputParameters);

        // Для хранимок, которые возвращают массив результатов
        IEnumerable<T> ExecuteProcedureWithSelectListOf<T>(string procedureName, int? commandTimeout, params Tuple<string, object>[] inputParameters);
        IEnumerable<T> ExecuteProcedureWithPreeneratedIdsAndSelectListOf<T>(string procedureName, int? commandTimeout, IEnumerable<long> pregeneratedIds, params Tuple<string, object>[] inputParameters);

        Tuple<string, object>[] ExecuteProcedureWithOutputParameter(string procedureName, Tuple<string, object>[] inputParameters, Tuple<string, Type>[] outputParameterName);
        void ExecuteProcedure(string procedureName, params Tuple<string, object>[] inputParameters);
        void ExecuteProcedure(string procedureName, int commandTimeout, params Tuple<string, object>[] inputParameters);
        IList<IList<object>> ExecuteTableProcedure(string procedureName, int? commandTimeout, params Tuple<string, object>[] inputParameters);
        void ExecuteSqlString(string queryString);
    }
}
