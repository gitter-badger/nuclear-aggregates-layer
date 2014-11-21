using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    [Migration(13827, "Добавляем колонку RequestType в таблицу OrderProcessingRequests", "v.lapeev")]
    public sealed class Migration13827 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            // Миграция будет накатываться в релиз 1.3, все созданные заявки от ЛК могут быть только на продление
            // Поэтому по-умолчанию выставляем всем заявкам тип ПРОДЛЕНИЕ
            const string ProlongateRequestType = "1";
            const string RequestTypeColumnName = "RequestType";

            var table = context.Database.GetTable(ErmTableNames.OrderProcessingRequests);

            if (table == null)
            {
                return;
            }

            var columnCreator = GetIntNotNulColumnCreator(RequestTypeColumnName);

            var newColumns = new[]
                {
                    new InsertedNotNullableColumnDefinition(3, columnCreator, RequestTypeColumnName, ProlongateRequestType)
                };

            EntityCopyHelper.CopyAndInsertColumns(context.Database, table, newColumns);

            CorrectRequestType(context); 
        }

        private static void CorrectRequestType(IMigrationContext context)
        {
            // если миграция будет накатываться не на боевую базу, а на тестовую, в ней могут быть храниться заявки на создание(!) заказа
            // необходимо правильно выставить им тип - СОЗДАНИЕ ЗАКАЗА
            context.Connection.ExecuteNonQuery("UPDATE [Billing].[OrderProcessingRequests] SET RequestType = 2 WHERE BaseOrderId IS NULL");
        }

        private static Func<SqlSmoObject, Column> GetIntNotNulColumnCreator(string columnName)
        {
            return smo => new Column(smo, columnName, DataType.Int) { Nullable = false };
        }
    }
}
