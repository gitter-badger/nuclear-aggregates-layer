using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10455, "Замена OperationName значениями OperationIdentity")]
    public sealed class Migration10455 : TransactedMigration
    {
        private const string FillTemporary = "update {0}.{1} set Temporary = {2} where Operation = {3}";
        private const string FindInvalid = "select Operation, Descriptor from {0}.{1} where Temporary is null";
        private const string MoveStatement = "update {0}.{1} set Operation = Temporary";

        private static readonly IDictionary<int, int> Changes = new Dictionary<int, int>()
            {
                { 1, 23 }, //CreateOrUpdate
                { 2, 9 }, //Delete
                { 3, 16 }, //Qualify
                { 4, 4 }, //Assign
                { 5, 3 }, //Append
                { 6, 8 }, //Deactivate
                { 7, 2 }, //Activate
                { 8, 10 }, //Disqualify
                { 9, 24 }, //Copy
                { 10, 25 }, //Merge
                { 11, 5 }, //ChangeClient
                { 12, 30 }, //Create
                { 13, 31 }, //Update
                { 14, 32 }, // Withdrawal
                { 15, 33 }, // RevertWithdrawal
                { 100, 15105 }, //RemoveBargainFromOrder
                { 101, 16401 }, //EditFirmAddressAdditionalServices
                { 102, 12 }, //FileUpload
                { 103, 21901 }, //SetProfileAsMain
                { 104, 18601 }, //SelectToWhitelist
                { 105, 15106 }, //ChangeDeal
                { 106, 15109 }, //SetInspector
                { 107, 19801 }, //BindToOrder
                { 108, 15107 }, //RepairOutdated
                { 109, 15108 }, //CloseWithDenial
                { 110, 14601 }, //ImportFirmCards
                { 111, 6 }, //ChangeTerritory
                { 112, 14602 }, //ImportFirmsFromDgpp
            };

        protected override void ApplyOverride(IMigrationContext context)
        {
            ProcessTable(context, ErmTableNames.PerformedBusinessOperations);
            ProcessTable(context, ErmTableNames.BusinessOperationServices);
        }

        private void ProcessTable(IMigrationContext context, SchemaQualifiedObjectName name)
        {
            var table = GetTable(context, name);

            var temporaryColumn = new Column(table, "Temporary", DataType.Int);
            table.Columns.Add(temporaryColumn);
            table.Alter();

            // Заполняю временный столбец корректными новыми значениями номеров операций
            foreach (var change in Changes)
            {
                var statement = string.Format(FillTemporary, name.Schema, name.Name, change.Value, change.Key);
                context.Database.ExecuteNonQuery(statement);
            }

            // Убеждаюсь, что нет необработанных операций
            using (var result = context.Database.ExecuteWithResults(string.Format(FindInvalid, name.Schema, name.Name)))
            using (var reader = result.CreateDataReader())
            {
                if (reader.Read())
                {
                    var message = string.Format("Somebody created operation with key ({0}, {1}), which is not described in this migration",
                                                reader.GetInt32(0),
                                                reader.GetInt32(1));
                    throw new Exception(message);
                }
            }

            // Переношу значения из временного стобца в прежний
            context.Database.ExecuteNonQuery(string.Format(MoveStatement, name.Schema, name.Name));

            // Сношу временный столбец
            table.Columns["Temporary"].Drop();
            table.Alter();
        }

        private Table GetTable(IMigrationContext context, SchemaQualifiedObjectName name)
        {
            return context.Database.Tables[name.Name, name.Schema];
        }
    }
}
