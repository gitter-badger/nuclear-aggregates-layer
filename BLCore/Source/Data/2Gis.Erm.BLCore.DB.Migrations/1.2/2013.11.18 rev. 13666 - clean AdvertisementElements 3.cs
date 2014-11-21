using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    /// <summary>
    /// В этой миграции приходится использовать SqlCommand, так как нельзя просто использовать string.Format для составления запросов:
    /// в текстовых рекламных материалах бывает что угодно и нужно экранировать потенциально опасные последовательности.
    /// </summary>
    [Migration(13666, "Удаление из AdvertisementElements спецсимволов")]
    public class Migration13666 : TransactedMigration
    {
        private const string SelectQuery = "select distinct Id, Text " +
                                           "from Billing.AdvertisementElements with(nolock) " +
                                           "where charindex(nchar(65279), AdvertisementElements.Text) <> 0";

        private const string UpdateAdvertisementElementCommandText = "update Billing.AdvertisementElements set Text = @text, ModifiedOn = @date, ModifiedBy = @admin where Id = @id";

        private const string InsertPerformedBusinessOperationCommandText = "insert into [Shared].[PerformedBusinessOperations]" +
                                                                           "([Id], [Operation], [Descriptor], [Context], [Date], [Parent]) " +
                                                                           "values (@id, 31, 237181970, @context, @date, null)";

        private const string OperationContextTemplate = "<context><entity change=\"3\" type=\"187\" id=\"{0}\" /></context>";

        private const long CrmAdministratorUserId = 1;

        private static readonly long[] Ids = new[]
            {
                232809347464298504, 232809414313115912, 232809460576289288, 232809501865018120, 232809548396626952,
            };

        private int _idPointer;

        protected override void ApplyOverride(IMigrationContext context)
        {
            var results = new Dictionary<long, string>();

            // В БД есть несколько записей с одним из символов BOM,
            // БД их воспринимает как символ с кодом 0 либо по настоящему коду. .Net только по настоящему.
            using (var reader = context.Connection.ExecuteReader(SelectQuery))
            {
                while (reader.Read())
                {
                    var sourceText = reader.GetString(1);

                    results.Add(reader.GetInt64(0), sourceText.Remove(sourceText.IndexOf((char)65279), 1));
                }
            }

            // Создаём своё подключение (System.Data.SqlClient) и собственную транзакцию, потому что на новое подключение уже существующая транзакция не распространяется.
            var migrationExecutionUtcDate = DateTime.UtcNow;
            using (var connection = new SqlConnection(context.Connection.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Snapshot))
                {
                    var updateAdvertisementElement = new SqlCommand(UpdateAdvertisementElementCommandText, connection, transaction);
                    updateAdvertisementElement.Parameters.Add(new SqlParameter("id", SqlDbType.BigInt));
                    updateAdvertisementElement.Parameters.Add(new SqlParameter("text", SqlDbType.NVarChar));
                    updateAdvertisementElement.Parameters.Add(new SqlParameter("date", SqlDbType.DateTime2));
                    updateAdvertisementElement.Parameters.Add(new SqlParameter("admin", SqlDbType.BigInt));

                    var insertPerformedBusinessOperation = new SqlCommand(InsertPerformedBusinessOperationCommandText, connection, transaction);
                    insertPerformedBusinessOperation.Parameters.Add(new SqlParameter("id", SqlDbType.BigInt));
                    insertPerformedBusinessOperation.Parameters.Add(new SqlParameter("context", SqlDbType.NVarChar));
                    insertPerformedBusinessOperation.Parameters.Add(new SqlParameter("date", SqlDbType.DateTime2));

                    foreach (var result in results)
                    {
                        try
                        {
                            updateAdvertisementElement.Parameters["id"].Value = result.Key;
                            updateAdvertisementElement.Parameters["text"].Value = result.Value;
                            updateAdvertisementElement.Parameters["date"].Value = migrationExecutionUtcDate;
                            updateAdvertisementElement.Parameters["admin"].Value = CrmAdministratorUserId;
                            updateAdvertisementElement.ExecuteNonQuery();

                            insertPerformedBusinessOperation.Parameters["id"].Value = GetNextId();
                            insertPerformedBusinessOperation.Parameters["context"].Value = string.Format(OperationContextTemplate, result.Key);
                            insertPerformedBusinessOperation.Parameters["date"].Value = migrationExecutionUtcDate;
                            insertPerformedBusinessOperation.ExecuteNonQuery();
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            var message = string.Format("Похоже, не хватило идентификаторов. Всего записей: {0}, всего идентификаторов: {1}", results.Count, Ids.Length);
                            throw new Exception(message, e);
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        private long GetNextId()
        {
            return Ids[_idPointer++];
        }
    }
}