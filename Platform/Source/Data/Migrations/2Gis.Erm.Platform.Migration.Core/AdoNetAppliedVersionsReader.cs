using System;
using System.Data;
using System.Data.SqlClient;

namespace DoubleGis.Erm.Platform.Migration.Core
{
    public class AdoNetAppliedVersionsReader : IAppliedVersionsReader
    {
        private readonly string _connectionString;

        public AdoNetAppliedVersionsReader(string connectionString)
        {
            _connectionString = connectionString;
            AppliedVersionsInfo = new AppliedVersionsInfo();
        }

        public AppliedVersionsInfo AppliedVersionsInfo { get; private set; }

        public void LoadVersionInfo()
        {
            AppliedVersionsInfo = new AppliedVersionsInfo();

            string sqlCommand = string.Format("SELECT Version FROM Shared.Migrations");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(sqlCommand, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AppliedVersionsInfo.AddAppliedMigration((long)reader[0]);
                        }
                    }
                }
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message, exception);
            }
        }
    }
}
