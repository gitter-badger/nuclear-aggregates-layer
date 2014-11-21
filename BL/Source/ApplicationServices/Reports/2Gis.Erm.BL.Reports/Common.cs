using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace DoubleGis.Erm.BL.Reports
{
    /// <summary>
    /// Значения параметров и строк подключения по умолчанию
    /// В основном для удобства разработки и тестирования
    /// </summary>
    internal static class Common
    {
        public static readonly long CITY = 0;
        public static readonly long CURRENT_USER = 0;
        public static readonly DateTime ISSUEDATE = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(-1);

        public static readonly SqlConnection WAREHOUSE_CONNECTION =
            new SqlConnection("Data Source=localhost;Initial Catalog=WH1;Integrated Security=True;");

        public static readonly SqlConnection ERM_CONNECTION =
            new SqlConnection(@"Data Source=uk-sql20\erm;Initial Catalog=ErmRU;Integrated Security=True");

        /// <summary>
        /// Получает текст внедренного ресурса по имени
        /// </summary>
        internal static string GetEmbeddedResourceText(string resourceName)
        {
            return (new StreamReader(GetEmbeddedResourceStream(resourceName), Encoding.Default)).ReadToEnd();
        }

        /// <summary>
        /// Получает поток внедренного ресурса по имени
        /// </summary>
        internal static Stream GetEmbeddedResourceStream(string resourceName)
        {
            var assembly = typeof(IReport).Assembly;
            var resourceFullName = assembly.GetManifestResourceNames().Single(item => item.EndsWith(resourceName));
            return assembly.GetManifestResourceStream(resourceFullName);
        }

        internal static void ExecuteNonQuery(SqlConnection connection, ProtectedDictionary parameters, string embeddedResourceName)
        {
            var command = connection.CreateCommand();
            command.CommandText = GetEmbeddedResourceText(embeddedResourceName);
            command.CommandTimeout = 0;
            parameters.SetCommandParameters(command);
            command.ExecuteNonQuery();
        }

        internal static DataTable ExecuteDataTable(SqlConnection connection, ProtectedDictionary parameters, string embeddedResourceName)
        {
            using (var dataTable = new DataTable())
            {
                var command = connection.CreateCommand();
                command.CommandText = GetEmbeddedResourceText(embeddedResourceName);
                command.CommandTimeout = 0;
                parameters.SetCommandParameters(command);
                var dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        internal static SqlDataReader ExecuteReader(SqlConnection connection, ProtectedDictionary parameters, string embeddedResourceName)
        {
            var command = connection.CreateCommand();
            command.CommandText = GetEmbeddedResourceText(embeddedResourceName);
            command.CommandTimeout = 0;
            parameters.SetCommandParameters(command);
            return command.ExecuteReader();

        }
    }
}
