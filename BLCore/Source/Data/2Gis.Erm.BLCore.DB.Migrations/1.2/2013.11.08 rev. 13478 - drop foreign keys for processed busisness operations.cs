using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13478, "Удаление FK из таблиц обработанных операций")]
    public sealed class Migration13478 : TransactedMigration
    {
        private static readonly ForeignKey[] KeysToRemove = new []
            {
                new ForeignKey("Integration", "ExportFlowCardExtensions_CardCommercial", "FK_ExportFlowCardExtensions_CardCommercial_PerformedBusinessOperations"),
                new ForeignKey("Integration", "ExportFlowFinancialData_Client", "FK_ExportFlowFinancialData_Client_PerformedBusinessOperations"),
                new ForeignKey("Integration", "ExportFlowFinancialData_LegalEntity", "FK_ExportFlowFinancialData_LegalEntity_PerformedBusinessOperations"),
                new ForeignKey("Integration", "ExportFlowOrders_AdvMaterial", "FK_ExportFlowOrders_AdvMaterial_PerformedBusinessOperations"),
                new ForeignKey("Integration", "ExportFlowOrders_Order", "FK_ExportFlowOrders_Order_PerformedBusinessOperations"),
                new ForeignKey("Integration", "ExportFlowOrders_Resource", "FK_ExportFlowOrders_Resource_PerformedBusinessOperations"),
                new ForeignKey("Integration", "ExportFlowOrders_Theme", "FK_ExportFlowOrders_Theme_PerformedBusinessOperations"),
                new ForeignKey("Integration", "ExportFlowOrders_ThemeBranch", "FK_ExportFlowOrders_ThemeBranch_PerformedBusinessOperations"),
                new ForeignKey("Integration", "ExportToMsCrm_HotClients", "FK_ExportToMsCrm_HotClients_PerformedBusinessOperations"),
                new ForeignKey("Integration", "ImportedFirmAddresses", "FK_ImportedFirmAddresses_PerformedBusinessOperations"),
            };

        protected override void ApplyOverride(IMigrationContext context)
        {
            foreach (var keyName in KeysToRemove)
            {
                var table = context.Database.Tables[keyName.TableName, keyName.TableSchema];
                if (table == null)
                {
                    throw new Exception(string.Format("Не найдена таблица {0}.{1}", keyName.TableSchema, keyName.TableName));
                }

                var key = table.ForeignKeys[keyName.ConstraintName];
                if (key == null)
                {
                    continue;
                }

                key.Drop();
            }
        }

        private class ForeignKey
        {
            public ForeignKey(string tableSchema, string tableName, string constraintName)
            {
                TableSchema = tableSchema;
                TableName = tableName;
                ConstraintName = constraintName;
            }

            public string TableSchema { get; private set; }
            public string TableName { get; private set; }
            public string ConstraintName { get; private set; }
        }
    }
}
