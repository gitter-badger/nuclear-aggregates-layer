using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Dapper;

namespace DoubleGis.Erm.Platform.DAL.AdoNet
{
    public sealed class AdoNetDatabaseCaller : IDatabaseCaller
    {
        private const string ReturnValueParameterName = "ReturnValue";
        private readonly string _connectionString;

        public AdoNetDatabaseCaller(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ExecuteProcedure(string procedureName, params Tuple<string, object>[] inputParameters)
        {
            ExecuteStoredProcedure(procedureName, null, inputParameters, null);
        }

        public void ExecuteProcedure(string procedureName, TimeSpan commandTimeout, params Tuple<string, object>[] inputParameters)
        {
            ExecuteStoredProcedure(procedureName, (int)commandTimeout.TotalSeconds, inputParameters, null);
        }

        public T ExecuteProcedureWithResultSingleValue<T>(string procedureName, params Tuple<string, object>[] inputParameters)
        {
            return ExecuteProcedureWithResultSingleValue<T>(procedureName, null, inputParameters);
        }

        public T ExecuteProcedureWithResultSingleValue<T>(string procedureName, TimeSpan? commandTimeout, params Tuple<string, object>[] inputParameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    FillInputParameters(command, inputParameters);
                    if (commandTimeout.HasValue)
                    {
                        command.CommandTimeout = (int)commandTimeout.Value.TotalSeconds;
                    }

                    connection.Open();

                    return Cast<T>(command.ExecuteScalar());
                }
            }
            catch (SqlException exception)
            {
                throw new ErmDataAccessException(_connectionString, procedureName, inputParameters, exception);
            }
        }

        public IEnumerable<T> ExecuteProcedureWithResultSequenceOf<T>(string procedureName, TimeSpan? commandTimeout, params Tuple<string, object>[] inputParameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    FillInputParameters(command, inputParameters);
                    if (commandTimeout.HasValue)
                    {
                        command.CommandTimeout = (int)commandTimeout.Value.TotalSeconds;
                    }

                    connection.Open();

                    var list = new List<T>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var value = reader.GetValue(0);
                            list.Add(Cast<T>(value));
                        }
                    }

                    return list;
                }
            }
            catch (SqlException exception)
            {
                throw new ErmDataAccessException(_connectionString, procedureName, inputParameters, exception);
            }
        }

        public DataTable ExecuteProcedureWithResultTable(string procedureName, TimeSpan? commandTimeout, params Tuple<string, object>[] inputParameters)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    FillInputParameters(command, inputParameters);
                    if (commandTimeout.HasValue)
                    {
                        command.CommandTimeout = (int)commandTimeout.Value.TotalSeconds;
                    }

                    connection.Open();

                    DataTable resultSet = null;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (resultSet == null)
                            {
                                resultSet = new DataTable();
                                for (var i = 0; i < reader.FieldCount; i++)
                                {
                                    var column = new DataColumn(reader.GetName(i));
                                    column.DataType = reader.GetFieldType(i);
                                    resultSet.Columns.Add(column);
                                }
                            }
                            
                            var newRow = resultSet.NewRow();
                            var rowValues = new object[reader.FieldCount];
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                rowValues[i] = reader.GetValue(i);
                            }

                            newRow.ItemArray = rowValues;
                            resultSet.Rows.Add(newRow);
                        }
                    }

                    return resultSet ?? new DataTable();
                }
            }
            catch (SqlException exception)
            {
                throw new ErmDataAccessException(_connectionString, procedureName, inputParameters, exception);
            }
        }

        public Tuple<string, object>[] ExecuteProcedureWithResultOutputParameters(string procedureName, Tuple<string, object>[] inputParameters, Tuple<string, Type>[] outputParameters)
        {
            var resultAccessor =
                new ProcedureResultAccessor(
                    outputParameters.Select(
                        x => new SqlParameter { ParameterName = x.Item1, Direction = ParameterDirection.Output }).ToArray());
            ExecuteStoredProcedure(procedureName, null, inputParameters, resultAccessor);
            return resultAccessor.ProcedureResult != null ?
                resultAccessor.ProcedureResult.Select(x => new Tuple<string, object>(x.ParameterName, x.Value)).ToArray()
                : new Tuple<string, object>[0];
        }

        public void ExecuteRawSql(string queryString)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var sqlCommand = new SqlCommand(queryString, connection))
            {
                connection.Open();
                sqlCommand.ExecuteNonQuery();
            }
        }

        public int ExecuteRawSql(string queryString, object parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Execute(queryString, parameters);
            }
        }

        public IEnumerable<TResult> QueryRawSql<TResult>(string queryString, object parameters = null, IDbConnection existingConnection = null, IDbTransaction transaction = null)
        {
            var connection = existingConnection ?? new SqlConnection(_connectionString);
            return connection.Query<TResult>(queryString, parameters, transaction);
        }

        public T ExecuteProcedureWithReturnValue<T>(string procedureName, object parameters, IDbConnection existingConnection = null, IDbTransaction transaction = null)
        {
            var connection = existingConnection ?? new SqlConnection(_connectionString);

            var p = new DynamicParameters(parameters);
            p.Add(ReturnValueParameterName, direction: ParameterDirection.ReturnValue);
            connection.Execute(procedureName, p, transaction, commandType: CommandType.StoredProcedure);

            return p.Get<T>(ReturnValueParameterName);
        }

        private static T Cast<T>(object value)
        {
            if (IsNullable(typeof(T)))
            {
                if (value == null || value is DBNull)
                {
                    return default(T); // то есть Null, только его тут явно нельза записать
                }

                // Convert.ChangeType не может привести к nullable-типу, зато с этим справляется explicit cast
                return (T)Convert.ChangeType(value, GetTypeFromNullable(typeof(T)));
            }

            if (value == null || value is DBNull)
            {
                throw new InvalidCastException();
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        private static Type GetTypeFromNullable(Type type)
        {
            return type.GetGenericArguments().Single();
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        
        private void FillInputParameters(SqlCommand command, IEnumerable<Tuple<string, object>> inputParameters)
        {
            foreach (var parameterDescriptor in inputParameters)
            {
                var sqlParameter = new SqlParameter
                {
                    ParameterName = parameterDescriptor.Item1,
                    Value = parameterDescriptor.Item2,
                    Direction = ParameterDirection.Input
                };

                if (parameterDescriptor.Item2 is DataTable)
                {
                    sqlParameter.SqlDbType = SqlDbType.Structured;
                }

                command.Parameters.Add(sqlParameter);
            }
        }

        private void ExecuteStoredProcedure(
            string procedureName,
            int? commandTimeout,
            Tuple<string, object>[] inputParameters,
            ProcedureResultAccessor procedureResultAccessor)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        FillInputParameters(command, inputParameters);
                        if (commandTimeout.HasValue)
                        {
                            command.CommandTimeout = commandTimeout.Value;
                        }

                        SqlParameter[] procedureResult = null;
                        if (procedureResultAccessor != null)
                        {
                            procedureResult = procedureResultAccessor.ProcedureObtainingResultSettings;
                            command.Parameters.AddRange(procedureResult);
                        }

                        connection.Open();
                        command.ExecuteNonQuery();

                        if (procedureResultAccessor != null)
                        {
                            procedureResultAccessor.ProcedureResult = procedureResult;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new ErmDataAccessException(_connectionString, procedureName, inputParameters, exception);
            }
        }

        private sealed class ProcedureResultAccessor
        {
            private readonly SqlParameter[] _procedureObtainingResultSettings;

            public ProcedureResultAccessor(SqlParameter[] procedureObtainingResultSettings)
            {
                _procedureObtainingResultSettings = procedureObtainingResultSettings;
            }

            public SqlParameter[] ProcedureObtainingResultSettings
            {
                get { return _procedureObtainingResultSettings; }
            }

            public SqlParameter[] ProcedureResult { get; set; }
        }
    }
}
