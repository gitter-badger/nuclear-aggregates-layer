using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

using OfficeOpenXml;

namespace DoubleGis.Erm.BL.Reports
{
    public abstract class ReportBase : IReport
    {
        #region private members

        private T GetSingleItemOfType<T>(IEnumerable<KeyValuePair<string, object>> dictionary)
        {
            return (T)dictionary.Single(item => item.Value is T).Value;
        }

        #endregion

        #region constructors

        protected ReportBase(Dictionary<string, object> parameters, Dictionary<string, object> connections)
        {
            Parameters = new ProtectedDictionary(parameters);
            Connections = new ProtectedDictionary(connections);
        }

        #endregion

        #region public members

        public abstract string ReportName { get; }

        public ProtectedDictionary Connections { get; protected set; }

        public ProtectedDictionary Parameters { get; protected set; }

        public abstract void SaveAs(string filePath);
        public abstract Stream ExecuteStream();

        #endregion

        #region protected members



        /// <summary>
        /// Метод реализует всю логику отчета, в нем должны произвестись все необходимые действия для заполнения отчета данными и их форматирования
        /// </summary>
        /// <returns></returns>
        protected abstract ExcelPackage Execute();

        protected DataTable ExecuteDataTable(string resourceName)
        {
            var sqlConnection = GetSingleItemOfType<SqlConnection>(Connections);
            return this.ExecuteDataTable(sqlConnection, resourceName);
        }

        protected void ExecuteNonQuery(string resourceName)
        {
            var sqlConnection = GetSingleItemOfType<SqlConnection>(Connections);
            this.ExecuteNonQuery(sqlConnection, resourceName);
        }

        #endregion

        public virtual ProtectedDictionary GetReformattedParameters()
        {
            return Parameters;
        }
    }
}
