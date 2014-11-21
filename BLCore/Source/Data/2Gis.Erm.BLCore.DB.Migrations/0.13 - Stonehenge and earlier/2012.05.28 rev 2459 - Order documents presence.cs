using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2459, "Задолженность по документам. Предварительно необходимо обновить объект 'dg_order' в CRM.")]
    public class Migration2459 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var ordersTable = context.Database.GetTable(ErmTableNames.Orders);

            if (ordersTable.Columns["DocumentsComment"] == null)
            {
                context.Connection.StatementTimeout = 300;

                Int32? hasDocumentDebtColumnIndex = ordersTable.Columns.IndexOf("HasDocumentsDebt");
                if (hasDocumentDebtColumnIndex.HasValue)
                {
                    Column hasDocumentDebtColumn = ordersTable.Columns[hasDocumentDebtColumnIndex.Value];

                    String defaultConstraintName = "DF_Orders_HasDocumentDebt";
                    if(hasDocumentDebtColumn.DefaultConstraint != null)
                    {
                        defaultConstraintName = hasDocumentDebtColumn.DefaultConstraint.Name;
                        hasDocumentDebtColumn.DefaultConstraint.Drop();
                    }

                    hasDocumentDebtColumn.DataType = DataType.TinyInt;

                    hasDocumentDebtColumn.AddDefaultConstraint(defaultConstraintName);
                    hasDocumentDebtColumn.DefaultConstraint.Text = "1";

                    ordersTable.Alter();

                    InsertedColumnDefinition documentsCommentColumn = new InsertedColumnDefinition
                        (hasDocumentDebtColumnIndex.Value + 1,
                         t => new Column(t, "DocumentsComment", DataType.NVarChar(300)) {Nullable = true});

                    EntityCopyHelper.CopyAndInsertColumns(context.Database, ordersTable,
                        new List<InsertedColumnDefinition> {documentsCommentColumn});

                    // Обновляем колонку: 
                    // 0 => OrderHasDocumentsDebt.Original = 2
                    // 1 => OrderHasDocumentsDebt.Absent = 1
                    String query = String.Format("UPDATE {0} SET HasDocumentsDebt = 2 WHERE HasDocumentsDebt = 0", ordersTable);
                    context.Connection.ExecuteNonQuery(query);
                }

                UpdateReplicateOrdersStoredProcedure(context);
                // TODO : replicate all orders.
            }
        }

        private void UpdateReplicateOrdersStoredProcedure(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            SchemaQualifiedObjectName storedProcedureName = new SchemaQualifiedObjectName(ErmSchemas.Billing, "ReplicateOrder");

            Assembly a = Assembly.GetAssembly(GetType());

            // Получить ресурс, приложенный к миграции.
            var stream = a.GetManifestResourceStream(GetType().FullName);
            if(stream == null)
                throw new InvalidOperationException("Failed to retrieve embedded resource.");
            StreamReader reader = new StreamReader(stream);
            String replicateOrderTemplate = reader.ReadToEnd();
            
            var spTextBody = string.Format(replicateOrderTemplate, context.CrmDatabaseName);

            try
            {
                ReplicationHelper.UpdateOrCreateReplicationSP(context, storedProcedureName, spTextBody);
            }
            catch(FailedOperationException ex)
            {
                Exception innerException;
                if (ex.RecursiveSearchMessage("\"Dg_documentsdebt\"", out innerException))
                {
                    throw new MigrationKnownException("Не удалось применить миграцию. Вероятно, не обновлены сущности CRM.", innerException);
                }
                throw;
            }
        }

    }
}
