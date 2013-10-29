using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.AdoNet
{
    public sealed class AdoNetDatabaseCaller : IDatabaseCaller
    {
        private readonly string _connectionString;

        public AdoNetDatabaseCaller(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Implementation of IDatabaseCaller

        public T ExecuteProcedureWithSelectValue<T>(string procedureName, params Tuple<string, object>[] inputParameters)
        {
            return ExecuteProcedureWithSelectValue<T>(procedureName, null, inputParameters);
        }

        public T ExecuteProcedureWithSelectValue<T>(string procedureName, int? commandTimeout, params Tuple<string, object>[] inputParameters)
        {
            var commandParametrs = inputParameters.Select(x => new SqlParameter { ParameterName = x.Item1, Value = x.Item2, Direction = ParameterDirection.Input }).ToArray();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(commandParametrs);
                if (commandTimeout.HasValue)
                {
                    command.CommandTimeout = commandTimeout.Value;
                }

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var value = default(object);
                    if (reader.Read())
                    {
                        value = reader.GetValue(0);
                    }

                    return Cast<T>(value);
                }
            }
        }

        public IEnumerable<T> ExecuteProcedureWithSelectListOf<T>(string procedureName, int? commandTimeout, params Tuple<string, object>[] inputParameters)
        {
            var commandParametrs = inputParameters.Select(x => new SqlParameter { ParameterName = x.Item1, Value = x.Item2, Direction = ParameterDirection.Input }).ToArray();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(commandParametrs);
                if (commandTimeout.HasValue)
                {
                    command.CommandTimeout = commandTimeout.Value;
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

        public IEnumerable<T> ExecuteProcedureWithPreeneratedIdsAndSelectListOf<T>(
            string procedureName,
            int? commandTimeout,
            IEnumerable<long> pregeneratedIds,
            params Tuple<string, object>[] inputParameters)
        {
            var idsTable = new DataTable("PregenaratedIds");
            idsTable.Columns.Add("Id", typeof(long));
            foreach (var id in pregeneratedIds)
            {
                idsTable.Rows.Add(id);
            }

            var commandParametrs =
                inputParameters.Select(x => new SqlParameter { ParameterName = x.Item1, Value = x.Item2, Direction = ParameterDirection.Input }).ToList();
            commandParametrs.Add(new SqlParameter { ParameterName = "PregenaratedIds", Value = idsTable, Direction = ParameterDirection.Input });
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(procedureName, connection))
            {

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(commandParametrs.ToArray());
                if (commandTimeout.HasValue)
                {
                    command.CommandTimeout = commandTimeout.Value;
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

        public T ExecuteProcedureWithReturnValue<T>(string procedureName, params Tuple<string, object>[] inputParameters)
        {
            return ExecuteProcedureWithReturnValue<T>(procedureName, null, inputParameters);
        }

        public T ExecuteProcedureWithReturnValue<T>(string procedureName, int? commandTimeout, params Tuple<string, object>[] inputParameters)
        {
            var commandParametrs = inputParameters.Select(x => new SqlParameter { ParameterName = x.Item1, Value = x.Item2, Direction = ParameterDirection.Input }).ToArray();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(commandParametrs);
                if (commandTimeout.HasValue)
                {
                    command.CommandTimeout = commandTimeout.Value;
                }

                connection.Open();

                return Cast<T>(command.ExecuteScalar());
            }
        }

        public IList<IList<object>> ExecuteTableProcedure(string procedureName, int? commandTimeout, params Tuple<string, object>[] inputParameters)
        {
            try
            {
                var commandParametrs = inputParameters.Select(x => new SqlParameter { ParameterName = x.Item1, Value = x.Item2, Direction = ParameterDirection.Input }).ToArray();
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (commandTimeout.HasValue)
                    {
                        command.CommandTimeout = commandTimeout.Value;
                    }

                    command.Parameters.AddRange(commandParametrs);

                    connection.Open();

                    var result = new List<IList<object>>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new List<object>(reader.FieldCount);
                            result.Add(row);
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.GetValue(i);
                                var isNull = value is DBNull;
                                row.Add(isNull ? null : value);
                            }
                        }
                    }

                    return result;
                }
            }
            catch (SqlException exception)
            {
                throw new ErmDataAccessException(_connectionString, procedureName, inputParameters, exception);
            }
        }

        public Tuple<string, object>[] ExecuteProcedureWithOutputParameter(string procedureName, Tuple<string, object>[] inputParameters, Tuple<string, Type>[] outputParameterName)
        {
            var resultAccessor =
                new ProcedureResultAccessor(
                    outputParameterName.Select(
                        x => new SqlParameter { ParameterName = x.Item1, Direction = ParameterDirection.Output }).ToArray());
            ExecuteStoredProcedure(procedureName, null, inputParameters, resultAccessor);
            return resultAccessor.ProcedureResult != null ?
                resultAccessor.ProcedureResult.Select(x => new Tuple<string, object>(x.ParameterName, x.Value)).ToArray()
                : new Tuple<string, object>[0];
        }

        public void ExecuteProcedure(string procedureName, params Tuple<string, object>[] inputParameters)
        {
            ExecuteStoredProcedure(procedureName, null, inputParameters, null);
        }

        public void ExecuteProcedure(string procedureName, int commandTimeout, params Tuple<string, object>[] inputParameters)
        {
            ExecuteStoredProcedure(procedureName, commandTimeout, inputParameters, null);
        }

        public void ExecuteSqlString(string queryString)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var sqlCommand = new SqlCommand(queryString, connection))
            {
                sqlCommand.ExecuteNonQuery();
            }
        }

        public T ExecuteInConnection<T>(Func<DbConnection, T> func)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return func.Invoke(connection);
            }
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

        private void ExecuteStoredProcedure(
            string procedureName,
            int? commandTimeout,
            IEnumerable<Tuple<string, object>> inputParameters,
            ProcedureResultAccessor procedureResultAccessor)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        if (commandTimeout.HasValue)
                        {
                            command.CommandTimeout = commandTimeout.Value;
                        }

                        var commandParametrs =
                            inputParameters.Select(x => new SqlParameter { ParameterName = x.Item1, Value = x.Item2, Direction = ParameterDirection.Input })
                                           .ToArray();
                        command.Parameters.AddRange(commandParametrs);

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
            catch (Exception e)
            {
                throw new StoredProcedureCallException(e, procedureName, inputParameters);
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

        #endregion
    }
}
