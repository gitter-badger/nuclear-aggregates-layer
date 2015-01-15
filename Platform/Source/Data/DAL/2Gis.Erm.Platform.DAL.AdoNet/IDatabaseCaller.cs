using System;
using System.Collections.Generic;
using System.Data;

namespace DoubleGis.Erm.Platform.DAL.AdoNet
{
    public interface IDatabaseCaller
    {
        // Использовать для хранимок, в которых результат возвращается через return.
        // В этих хранимках тип возвращаемого результата ограничен числовыми типами и не может быть Null.
        T ExecuteProcedureWithResultSingleValue<T>(string procedureName, params Tuple<string, object>[] inputParameters);
        T ExecuteProcedureWithResultSingleValue<T>(string procedureName, TimeSpan? commandTimeout, params Tuple<string, object>[] inputParameters);

        // Для хранимок, которые возвращают массив результатов
        IEnumerable<T> ExecuteProcedureWithResultSequenceOf<T>(string procedureName, TimeSpan? commandTimeout, params Tuple<string, object>[] inputParameters);
        
        Tuple<string, object>[] ExecuteProcedureWithResultOutputParameters(string procedureName, Tuple<string, object>[] inputParameters, Tuple<string, Type>[] outputParameters);
        void ExecuteProcedure(string procedureName, params Tuple<string, object>[] inputParameters);
        void ExecuteProcedure(string procedureName, TimeSpan commandTimeout, params Tuple<string, object>[] inputParameters);
        DataTable ExecuteProcedureWithResultTable(string procedureName, TimeSpan? commandTimeout, params Tuple<string, object>[] inputParameters);
        void ExecuteRawSql(string queryString);
        int ExecuteRawSql(string queryString, object parameters);
        IEnumerable<TResult> QueryRawSql<TResult>(string queryString, object parameters = null, IDbConnection existingConnection = null, IDbTransaction transaction = null);
        T ExecuteProcedureWithReturnValue<T>(string procedureName, object parameters, IDbConnection existingConnection = null, IDbTransaction transaction = null);
    }
}
